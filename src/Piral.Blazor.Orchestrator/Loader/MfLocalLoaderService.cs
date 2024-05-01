using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator.Loader;

internal class MfLocalLoaderService<T>(T originalLoader, IMfRepository repository, IModuleContainerService container, IEvents events, IData data) : IMfLoaderService
    where T : class, IMfLoaderService
{
    private readonly T _originalLoader = originalLoader;
    private readonly IMfRepository _repository = repository;
    private readonly IModuleContainerService _container = container;
    private readonly IEvents _events = events;
    private readonly IData _data = data;

    public void ConnectMicrofrontends(CancellationToken cancellationToken)
    {
        _originalLoader.ConnectMicrofrontends(cancellationToken);
    }

    public async Task LoadMicrofrontends(CancellationToken cancellationToken)
    {
        var all = (Environment.GetEnvironmentVariable("PIRAL_BLAZOR_ALL_DEBUG_ASSEMBLIES") ?? "").Split(',');

        foreach (var path in all)
        {
            var cfg = GetMicrofrontendConfig(path);
            await _repository.SetPackage(new LocalMicrofrontendPackage(path, cfg, _container, _events, _data));
        }
    }

    private static JsonObject? GetMicrofrontendConfig(string path)
    {
        var dir = Path.GetDirectoryName(path)!;
        var cfgPath = Path.Combine(dir, "config.json");

        if (File.Exists(cfgPath))
        {
            var text = File.ReadAllText(cfgPath, Encoding.UTF8);
            return JsonSerializer.Deserialize<JsonObject?>(text);
        }

        return null;
    }
}
