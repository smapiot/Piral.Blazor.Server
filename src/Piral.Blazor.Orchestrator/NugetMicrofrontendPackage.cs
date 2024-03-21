using NuGet.Frameworks;
using NuGet.Packaging;
using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Orchestrator;

internal class NugetMicrofrontendPackage(string name, string version, List<PackageArchiveReader> packages, IPiralConfig config, IModuleContainerService container, IEvents events, IData data) : MicrofrontendPackage(name, version, container, events, data)
{
    private const string target = "net8.0";
    private readonly IPiralConfig _config = config;
    private readonly Dictionary<string, PackageArchiveReader> _packages = packages.ToDictionary(m => m.NuspecReader.GetId());

    private Assembly? LoadAssembly(PackageArchiveReader package, string path)
    {
        using var msStream = GetFile(package, path).Result;

        if (msStream is not null)
        {
            return Context.LoadFromStream(msStream);
        }

        return null;
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

    protected override Assembly? LoadMissingAssembly(AssemblyLoadContext _, AssemblyName assemblyName)
    {
        var dll = $"{assemblyName.Name}.dll";
        return AddAssemblyToContext(dll);
    }

    private Assembly? AddAssemblyToContext(string dll)
    {
        foreach (var package in _packages.Values)
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

    protected override string GetCssName() => $"{Name}.bundle.scp.css";

    protected override Assembly? GetAssembly() => LoadAssembly(_packages[Name], $"lib/{target}/{Name}.dll");

    protected override Task OnInitializing()
    {
        foreach (var assembly in _config.IsolatedAssemblies)
        {
            AddAssemblyToContext(assembly);
        }

        return base.OnInitializing();
    }

    public override async Task<Stream?> GetFile(string path)
    {
        if (path.StartsWith("_content"))
        {
            var segments = path.Split('/');
            var packageName = segments[1];
            var localPath = string.Join('/', segments.Skip(2));
            var package = _packages[packageName];

            if (package is not null)
            {
                return await GetFile(package, $"staticwebassets/{localPath}");
            }

            return null;
        }
        else
        {
            var package = _packages[Name];
            return await GetFile(package, $"staticwebassets/{path}");
        }
    }
}
