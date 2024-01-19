using System.Reflection;

namespace Piral.Blazor.Orchestrator.Loader;

internal class MfLocalLoaderService<T>(T originalLoader, IMfRepository repository, IModuleContainerService container, IEvents events) : IMfLoaderService
    where T : class, IMfLoaderService
{
    private readonly T _originalLoader = originalLoader;
    private readonly IMfRepository _repository = repository;
    private readonly IModuleContainerService _container = container;
    private readonly IEvents _events = events;

    public void ConnectMicrofrontends(CancellationToken cancellationToken)
    {
        _originalLoader.ConnectMicrofrontends(cancellationToken);
    }

    public async Task LoadMicrofrontends(CancellationToken cancellationToken)
    {
        var ass = Assembly.GetEntryAssembly()!;
        var mf = new LocalMicrofrontendPackage(ass, _container, _events);
        await _repository.SetPackage(mf);
    }
}
