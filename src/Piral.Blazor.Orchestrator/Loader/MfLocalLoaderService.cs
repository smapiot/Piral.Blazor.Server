using System.Reflection;
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
        var ass = Assembly.GetEntryAssembly()!;
        var all = (Environment.GetEnvironmentVariable("PIRAL_BLAZOR_ALL_DEBUG_ASSEMBLIES") ?? "").Split(',');
        var cfg = new JsonObject();

        // set primary
        await _repository.SetPackage(new LocalMicrofrontendPackage(ass, cfg, _container, _events, _data));

        foreach (var path in all)
        {
            if (path != ass.Location)
            {
                // set other
                var other = Assembly.LoadFrom(path);
                await _repository.SetPackage(new LocalMicrofrontendPackage(other, cfg, _container, _events, _data));
            }
        }
    }
}
