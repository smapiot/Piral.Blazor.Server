using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Piral.Blazor.Orchestrator;

internal class LocalMicrofrontendPackage : MicrofrontendPackage
{
    private readonly Assembly _assembly;
    private readonly AssemblyLoadContext _context;
    private readonly List<string> _contentRoots = new();

    public LocalMicrofrontendPackage(Assembly assembly, IModuleContainerService container, IEvents events)
        : this(assembly, assembly.GetName(), container, events)
    {
    }

    private LocalMicrofrontendPackage(Assembly assembly, AssemblyName assemblyName, IModuleContainerService container, IEvents events)
        : base(assemblyName.Name!, assemblyName.Version!.ToString(), container, events)
    {
        _assembly = assembly;
        _context = new AssemblyLoadContext($"local_package", true);
        _context.Resolving += LoadMissingAssembly;
    }

    private Assembly? LoadMissingAssembly(AssemblyLoadContext _, AssemblyName assemblyName)
    {
        //TODO
        return null;
    }

    protected override async Task OnInitialized()
    {
        var infos = Path.ChangeExtension(_assembly.Location, ".staticwebassets.runtime.json");
        using var fs = File.OpenRead(infos);
        var assets = await JsonSerializer.DeserializeAsync<StaticWebAssets>(fs);
        _contentRoots.AddRange(assets?.ContentRoots ?? Enumerable.Empty<string>());
    }

    protected override Assembly? GetAssembly() => _context.LoadFromAssemblyPath(_assembly.Location);

    public override Stream? GetFile(string path)
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
                        return fs;
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
                    return fs;
                }
            }
        }

        return null;
    }

    protected override string GetCssName() => $"{Name}.styles.css";

    class StaticWebAssets
    {
        public List<string>? ContentRoots { get; set; }
    }
}
