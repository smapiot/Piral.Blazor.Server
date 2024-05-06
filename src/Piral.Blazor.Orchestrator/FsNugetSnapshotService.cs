using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public class FsNugetSnapshotService : ISnapshotService
{
    private readonly ConcurrentDictionary<string, PackageArchiveReader> _db = new();
    private readonly ConcurrentDictionary<string, IEnumerable<PackageMetadata>> _deps = new();
    private readonly ConcurrentQueue<Func<Task>> _jobs = new();
    private readonly List<MfPackageMetadata> _mfs = [];

    private readonly INugetService _nuget;
    private readonly DirectoryInfo _snapshot;
    private bool _initialized;
    private Task? _queue;

    public FsNugetSnapshotService(INugetService nuget, IConfiguration configuration)
    {
        var cacheDir = configuration.GetValue<string>("Microfrontends:CacheDir") ?? ".cache";
        _nuget = nuget;
        _snapshot = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, cacheDir));
        _initialized = false;
    }

    public Task UpdateMicrofrontends(IEnumerable<MfPackageMetadata> entries)
    {
        return EnqueueJob(() =>
        {
            _mfs.Clear();
            _mfs.AddRange(entries);
            return StoreMicrofrontendsSnapshot(entries);
        });
    }

    private Task EnqueueJob(Func<Task> process)
    {
        _jobs.Enqueue(process);

        if (_queue is null || _queue.Status == TaskStatus.RanToCompletion)
        {
            _queue = ProcessJobs();
        }

        return _queue;
    }

    public async Task<IEnumerable<string>> AvailableMicrofrontends()
    {
        if (!_initialized)
        {
            await RestorePackageSnapshot();
            _initialized = true;
        }

        return _mfs.Select(m => m.MakePackageId()).ToList();
    }

    public async Task<JsonObject?> GetConfig(string id)
    {
        if (!_initialized)
        {
            await RestorePackageSnapshot();
            _initialized = true;
        }

        var (name, version) = id.GetIdentity();
        return _mfs.Find(m => m.Name == name && m.Version == version)?.Config;
    }

    public async Task<PackageArchiveReader?> LoadPackage(string id)
    {
        if (!_initialized)
        {
            await RestorePackageSnapshot();
            _initialized = true;
        }

        if (!_db.TryGetValue(id, out var result))
        {
            var (name, version) = id.GetIdentity();
            result = await _nuget.DownloadPackage(name, version);

            if (result is not null)
            {
                _db.TryAdd(id, result);
                await StorePackageSnapshot(id, result);
            }
        }

        return result;
    }

    public async Task<IEnumerable<PackageMetadata>> ListDependencies(string id)
    {
        if (!_initialized)
        {
            await RestorePackageSnapshot();
            _initialized = true;
        }

        if (!_deps.TryGetValue(id, out var result))
        {
            var (name, version) = id.GetIdentity();
            result = await _nuget.RetrieveDependencies(name, version);

            if (result is not null)
            {
                _deps.TryAdd(id, result);
                await StoreDependenciesSnapshot(id, result);
            }
        }

        return result ?? [];
    }

    private async Task RestorePackageSnapshot()
    {
        var files = _snapshot.EnumerateFiles();

        foreach (var file in files)
        {
            var id = Path.GetFileNameWithoutExtension(file.Name);
            var ext = Path.GetExtension(file.Name);

            if (id == "_" && ext == ".json")
            {
                using var fs = file.OpenRead();
                var result = await JsonSerializer.DeserializeAsync<List<MfPackageMetadata>>(fs);
                _mfs.Clear();
                _mfs.AddRange(result ?? Enumerable.Empty<MfPackageMetadata>());
            }
            else if (ext == ".nupkg" && !_db.ContainsKey(id))
            {
                var stream = new MemoryStream();
                using var fs = file.OpenRead();
                await fs.CopyToAsync(stream);
                stream.Position = 0;
                var reader = new PackageArchiveReader(stream);
                _db.TryAdd(id, reader);
            }
            else if (ext == ".json" && !_deps.ContainsKey(id))
            {
                using var fs = file.OpenRead();
                var result = await JsonSerializer.DeserializeAsync<List<PackageMetadata>>(fs);
                _deps.TryAdd(id, result ?? Enumerable.Empty<PackageMetadata>());
            }
        }
    }

    private async Task StoreMicrofrontendsSnapshot(IEnumerable<MfPackageMetadata> microfrontends)
    {
        var fn = $"_.json";
        var target = Path.Combine(_snapshot.FullName, fn);
        using var fs = File.OpenWrite(target);
        await JsonSerializer.SerializeAsync(fs, microfrontends.ToList());
    }

    private async Task StorePackageSnapshot(string id, PackageArchiveReader reader)
    {
        var fn = $"{id}.nupkg";
        var target = Path.Combine(_snapshot.FullName, fn);
        await reader.CopyNupkgAsync(target, CancellationToken.None);
    }

    private async Task StoreDependenciesSnapshot(string id, IEnumerable<PackageMetadata> dependencies)
    {
        var fn = $"{id}.json";
        var target = Path.Combine(_snapshot.FullName, fn);
        using var fs = File.OpenWrite(target);
        await JsonSerializer.SerializeAsync(fs, dependencies.ToList());
    }

    private async Task ProcessJobs()
    {
        while (_jobs.TryDequeue(out var process))
        {
            try
            {
                await process();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Problem with snapshot job: {ex.Message}");
            }
        }
    }
}
