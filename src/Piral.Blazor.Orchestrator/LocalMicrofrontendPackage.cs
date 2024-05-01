using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

internal class LocalMicrofrontendPackage : MicrofrontendPackage
{
    private readonly string _path;
    private readonly List<string> _contentRoots = [];

    public LocalMicrofrontendPackage(string path, JsonObject? config, IModuleContainerService container, IEvents events, IData data)
        : base(Path.GetFileNameWithoutExtension(path), "0.0.0", config, container, events, data)
    {
        _path = path;
    }

    protected override Assembly? LoadMissingAssembly(AssemblyLoadContext _, AssemblyName assemblyName)
    {
        //TODO
        return null;
    }

    protected override async Task OnInitialized()
    {
        var infos = Path.ChangeExtension(_path, ".staticwebassets.runtime.json");
        using var fs = File.OpenRead(infos);
        var assets = await JsonSerializer.DeserializeAsync<StaticWebAssets>(fs);
        _contentRoots.AddRange(assets?.ContentRoots ?? Enumerable.Empty<string>());
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

    class StaticWebAssets
    {
        public List<string>? ContentRoots { get; set; }
    }
}
