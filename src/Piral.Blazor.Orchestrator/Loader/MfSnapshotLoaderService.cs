namespace Piral.Blazor.Orchestrator.Loader;

public class MfSnapshotLoaderService : IMfLoaderService
{
    private readonly IMfRepository _repository;
    private readonly IMfPackageService _package;
    private readonly ISnapshotService _snapshot;

    public MfSnapshotLoaderService(IMfRepository repository, IMfPackageService package, ISnapshotService snapshot)
    {
        _repository = repository;
        _package = package;
        _snapshot = snapshot;
    }

    public void ConnectMicrofrontends(CancellationToken cancellationToken)
    {
        // Empty on purpose
    }

    public async Task LoadMicrofrontends(CancellationToken cancellationToken)
    {
        var ids = await _snapshot.AvailableMicrofrontends();

        foreach (var id in ids)
        {
            var (name, version) = id.GetIdentity();
            var mf = await _package.LoadMicrofrontend(name, version);
            await _repository.SetPackage(mf);
        }
    }
}