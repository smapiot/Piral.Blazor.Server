namespace Piral.Blazor.Orchestrator;

public class MfRepository : IMfRepository, IDisposable
{
	private readonly List<MicrofrontendPackage> _microfrontends = new();
    private readonly ISnapshotService _snapshot;

    public event EventHandler? PackagesChanged;

    public MfRepository(ISnapshotService snapshot)
    {
        _snapshot = snapshot;
	}

	public IEnumerable<MicrofrontendPackage> Packages => _microfrontends;

    public async Task DeletePackage(string name)
    {
        var items = _microfrontends.Where(m => m.Name == name).ToList();

        if (items.Any())
        {
            foreach (var item in items)
            {
                _microfrontends.Remove(item);
                await item.Destroy();
                item.Dispose();
            }

            await Update();
        }
    }

    public async Task SetPackage(MicrofrontendPackage package)
    {
        var name = package.Name;
        var version = package.Version;

        if (!_microfrontends.Any(m => m.Name == name && m.Version == version))
        {
            await package.Init();
            _microfrontends.RemoveAll(m => m.Name == name);
            _microfrontends.Add(package);
            await Update();
        }
    }

    public void Dispose()
    {
        _microfrontends.ForEach(m => m.Dispose());
        _microfrontends.Clear();
    }

    private async Task Update()
    {
        var ids = _microfrontends.Select(m => new NugetEntry
        {
            Name = m.Name,
            Version = m.Version,
        }).Select(m => m.MakePackageId());

        await _snapshot.UpdateMicrofrontends(ids);

        PackagesChanged?.Invoke(this, EventArgs.Empty);
    }
}
