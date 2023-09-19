using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Piral.Blazor.Orchestrator;

public class FsNugetSnapshotService : ISnapshotService
{
    private readonly ConcurrentDictionary<string, PackageArchiveReader> _db = new();
    private readonly ConcurrentDictionary<string, IEnumerable<NugetEntry>> _deps = new();
    private readonly List<NugetEntry> _mfs = new();

    private readonly INugetService _nuget;
    private readonly DirectoryInfo _snapshot;
    private bool _initialized;

    public FsNugetSnapshotService(INugetService nuget, IConfiguration configuration)
    {
        var cacheDir = configuration.GetValue<string>("Microfrontends:CacheDir")!;
        _nuget = nuget;
        _snapshot = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, cacheDir));
        _initialized = false;
    }

    public async Task UpdateMicrofrontends(IEnumerable<string> ids)
    {
        var entries = ids.Select(m =>
        {
            var (name, version) = m.GetIdentity();
            return new NugetEntry { Name = name, Version = version };
        });
        _mfs.Clear();
        _mfs.AddRange(entries);
        await StoreMicrofrontendsSnapshot(entries);
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

    public async Task<IEnumerable<NugetEntry>> ListDependencies(string id)
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

        return result ?? Enumerable.Empty<NugetEntry>();
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
                var result = await JsonSerializer.DeserializeAsync<List<NugetEntry>>(fs);
                _mfs.Clear();
                _mfs.AddRange(result ?? Enumerable.Empty<NugetEntry>());
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
                var result = await JsonSerializer.DeserializeAsync<List<NugetEntry>>(fs);
                _deps.TryAdd(id, result ?? Enumerable.Empty<NugetEntry>());
            }
        }
    }

    private async Task StoreMicrofrontendsSnapshot(IEnumerable<NugetEntry> microfrontends)
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

    private async Task StoreDependenciesSnapshot(string id, IEnumerable<NugetEntry> dependencies)
    {
        var fn = $"{id}.json";
        var target = Path.Combine(_snapshot.FullName, fn);
        using var fs = File.OpenWrite(target);
        await JsonSerializer.SerializeAsync(fs, dependencies.ToList());
    }
}
