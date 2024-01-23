using NuGet.Frameworks;
using NuGet.Packaging;
using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Orchestrator;

internal class NugetMicrofrontendPackage : MicrofrontendPackage
{
    private const string target = "net8.0";
    private readonly ICacheManipulatorService _cacheManipulator;
    private readonly Dictionary<string, PackageArchiveReader> _packages;
    private readonly AssemblyLoadContext _context;
    private Assembly? _assembly;

    public NugetMicrofrontendPackage(string name, string version, List<PackageArchiveReader> packages, IModuleContainerService container, IEvents events, ICacheManipulatorService cacheManipulator)
        : base(name, version, container, events)
    {
        _cacheManipulator = cacheManipulator;
        _packages = packages.ToDictionary(m => m.NuspecReader.GetId());
        _context = new AssemblyLoadContext($"{name}@{version}", true);
        _context.Resolving += LoadMissingAssembly;
    }

    private Assembly? LoadAssembly(PackageArchiveReader package, string path)
    {
        using var msStream = GetFile(package, path);

        if (msStream is not null)
        {
            var assembly = _context.LoadFromStream(msStream);
            _cacheManipulator.UpdateComponentCache(assembly);
            return assembly;
        }

        return null;
    }

    private static Stream? GetFile(PackageArchiveReader package, string path)
    {
        try
        {
            var zip = package.GetEntry(path);

            if (zip is not null)
            {
                using var zipStream = zip.Open();
                var msStream = new MemoryStream();
                zipStream.CopyTo(msStream);
                msStream.Position = 0;
                return msStream;
            }
        }
        catch (FileNotFoundException)
        {
            // This is expected - nothing wrong here
        }

        return null;
    }

    private Assembly? LoadMissingAssembly(AssemblyLoadContext _, AssemblyName assemblyName)
    {
        var dll = $"{assemblyName.Name}.dll";

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

    protected override Assembly? GetAssembly() => _assembly ??= LoadAssembly(_packages[Name], $"lib/{target}/{Name}.dll");

    public override void Dispose() => _context.Unload();

    public override Stream? GetFile(string path)
    {
        if (path.StartsWith("_content"))
        {
            var segments = path.Split('/');
            var packageName = segments[1];
            var localPath = string.Join('/', segments.Skip(2));
            var package = _packages[packageName];

            if (package is not null)
            {
                return GetFile(package, $"staticwebassets/{localPath}");
            }

            return null;
        }
        else
        {
            var package = _packages[Name];
            return GetFile(package, $"staticwebassets/{path}");
        }
    }
}
