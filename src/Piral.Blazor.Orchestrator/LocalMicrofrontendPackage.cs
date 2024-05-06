using NuGet.Frameworks;
using NuGet.Packaging;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Piral.Blazor.Orchestrator;

internal class LocalMicrofrontendPackage(string path, JsonObject? config, IModuleContainerService container, IEvents events, IData data) :
    MicrofrontendPackage(new MfPackageMetadata { Name = Path.GetFileNameWithoutExtension(path), Version = "0.0.0", Config = config }, container, events, data)
{
    private const string target = "net8.0";
    private readonly string _path = path;
    private readonly List<string> _contentRoots = [];
    private readonly Dictionary<string, DependencyDescription> _deps = [];
    private readonly List<PackageArchiveReader> _packages = [];

    private Assembly? LoadAssembly(PackageArchiveReader package, string path)
    {
        using var msStream = GetFile(package, path).Result;

        if (msStream is not null)
        {
            return Context.LoadFromStream(msStream);
        }

        return null;
    }

    protected override Assembly? ResolveAssembly(AssemblyName assemblyName)
    {
        var dllName = assemblyName.Name!;
        var version = assemblyName.Version?.ToString()!;
        return ResolveAssembly(dllName, version);
    }

    private Assembly? ResolveAssembly(string name, string version)
    {
        var packageId = $"{name}/{version}";

        if (_deps.TryGetValue(packageId, out var dep) && dep.Type == "package")
        {
            var packageName = name.ToLowerInvariant();
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var packagePath = Path.Combine(userProfile, ".nuget", "packages", packageName, version, $"{packageName}.{version}.nupkg");
            var stream = File.OpenRead(packagePath);
            _packages.Add(new PackageArchiveReader(stream));
            return AddAssemblyToContext(name);
        }
        else
        {
            var basePath = Path.GetDirectoryName(_path)!;
            return Context.LoadFromAssemblyPath(Path.Combine(basePath, $"{name}.dll"));
        }
    }

    protected override async Task OnInitializing()
    {
        await SetContentRoots();
        await SetDependencies();
    }

    private async Task SetContentRoots()
    {
        var infos = Path.ChangeExtension(_path, ".staticwebassets.runtime.json");
        using var fs = File.OpenRead(infos);
        var assets = await JsonSerializer.DeserializeAsync<StaticWebAssets>(fs);

        if (assets?.ContentRoots is not null)
        {
            _contentRoots.AddRange(assets.ContentRoots);
        }
    }

    private async Task SetDependencies()
    {
        var infos = Path.ChangeExtension(_path, ".deps.json");
        using var fs = File.OpenRead(infos);
        var deps = await JsonSerializer.DeserializeAsync<DependenciesList>(fs);

        if (deps?.Libraries is not null)
        {
            _deps.AddRange(deps.Libraries);
        }
    }

    protected override Assembly? GetAssembly() => Context.LoadFromAssemblyPath(_path);

    public override Task<Stream?> GetFile(string path)
    {
        if (path.StartsWith("_content"))
        {
            var segments = path.Split('/');
            var packageName = segments[1];
            var localPath = string.Join('/', segments.Skip(2));

            foreach (var contentRoot in _contentRoots)
            {
                if (contentRoot.Contains(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    var fullPath = Path.Combine(contentRoot, localPath);

                    if (File.Exists(fullPath))
                    {
                        var fs = File.OpenRead(fullPath);
                        return Task.FromResult<Stream?>(fs);
                    }
                }
            }
        }
        else
        {
            foreach (var contentRoot in _contentRoots)
            {
                var fullPath = Path.Combine(contentRoot, path);

                if (File.Exists(fullPath))
                {
                    var fs = File.OpenRead(fullPath);
                    return Task.FromResult<Stream?>(fs);
                }
            }
        }

        return Task.FromResult<Stream?>(null);
    }

    protected override string GetCssName() => $"{Name}.styles.css";

    private Assembly? AddAssemblyToContext(string dll)
    {
        foreach (var package in _packages)
        {
            var libItems = package.GetLibItems().FirstOrDefault(m => IsCompatible(m.TargetFramework))?.Items;

            if (libItems is not null)
            {
                foreach (var lib in libItems)
                {
                    if (lib.EndsWith(dll))
                    {
                        return LoadAssembly(package, lib);
                    }
                }
            }
        }

        return null;
    }

    private static bool IsCompatible(NuGetFramework framework)
    {
        var current = NuGetFramework.Parse(target);
        return DefaultCompatibilityProvider.Instance.IsCompatible(current, framework);
    }

    private static async Task<MemoryStream?> GetFile(PackageArchiveReader package, string path)
    {
        try
        {
            var zip = package.GetEntry(path);

            if (zip is not null)
            {
                using var zipStream = zip.Open();
                var msStream = new MemoryStream();
                await zipStream.CopyToAsync(msStream);
                msStream.Position = 0;
                return msStream;
            }
        }
        catch (FileNotFoundException)
        {
            // This is expected - nothing wrong here
        }
        catch (InvalidDataException)
        {
            // This is not expected, but should be handled gracefully
        }

        return null;
    }

    class StaticWebAssets
    {
        public List<string>? ContentRoots { get; set; }
    }

    class DependenciesList
    {
        [JsonPropertyName("libraries")]
        public Dictionary<string, DependencyDescription>? Libraries { get; set; }
    }

    class DependencyDescription
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("serviceable")]
        public bool? IsServiceable { get; set; }

        [JsonPropertyName("sha512")]
        public string? SHA512 { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("hashPath")]
        public string? HashPath { get; set; }
    }
}
