namespace Piral.Blazor.Orchestrator.Loader;

internal class MfLocalLoaderService<T>(T originalLoader, IMfRepository repository, IPiralConfig config, IModuleContainerService container, IEvents events, IData data) : IMfLoaderService
    where T : class, IMfLoaderService
{
    private readonly T _originalLoader = originalLoader;
    private readonly IMfRepository _repository = repository;
    private readonly IModuleContainerService _container = container;
    private readonly IPiralConfig _config = config;
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
            await _repository.SetPackage(new LocalMicrofrontendPackage(path, _config, _container, _events, _data));
        }
    }
}
