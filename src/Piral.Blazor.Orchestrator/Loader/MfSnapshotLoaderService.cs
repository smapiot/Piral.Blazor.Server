namespace Piral.Blazor.Orchestrator.Loader;

public class MfSnapshotLoaderService(IMfRepository repository, IMfPackageService package, ISnapshotService snapshot) : IMfLoaderService
{
    private readonly IMfRepository _repository = repository;
    private readonly IMfPackageService _package = package;
    private readonly ISnapshotService _snapshot = snapshot;

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