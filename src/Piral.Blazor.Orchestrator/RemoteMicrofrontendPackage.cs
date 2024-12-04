using NuGet.Packaging;
using Piral.Blazor.Shared;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

internal class RemoteMicrofrontendPackage(MfPackageMetadata entry, List<PackageArchiveReader> packages, IPiralConfig config, IModuleContainerService container, IGlobalEvents events, IData data) :
    MicrofrontendPackage(entry, config, container, events, data)
{
    private readonly List<PackageArchiveReader> _packages = packages;

    private Assembly? LoadAssembly(PackageArchiveReader? package, string path)
    {
        if (package is not null)
        {
            using var msStream = GetFile(package, path).Result;

            if (msStream is not null)
            {
                return Context.LoadFromStream(msStream);
            }
        }

        return null;
    }

    private static async Task<MemoryStream?> GetFile(PackageArchiveReader? package, string path)
    {
        try
        {
            var zip = package?.GetEntry(path);

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

    protected override Assembly? ResolveAssembly(string dll)
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

    protected override string GetCssName() => $"{Name}.bundle.scp.css";

    protected override Assembly? GetAssembly() => LoadAssembly(FindPackage(Name), $"lib/{Constants.Target}/{Name}.dll");

    private PackageArchiveReader? FindPackage(string name)
    {
        return _packages.FirstOrDefault(m => m.NuspecReader.GetId() == name);
    }

    public override async Task<Stream?> GetFile(string path)
    {
        if (path.StartsWith("_content"))
        {
            var segments = path.Split('/');
            var packageName = segments[1];
            var localPath = string.Join('/', segments.Skip(2));
            var package = FindPackage(packageName);

            if (package is not null)
            {
                return await GetFile(package, $"staticwebassets/{localPath}");
            }

            return null;
        }

        return await GetFile(FindPackage(Name), $"staticwebassets/{path}");
    }
}
