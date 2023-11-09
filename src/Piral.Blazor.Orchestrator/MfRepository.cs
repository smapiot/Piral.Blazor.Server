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

    public MicrofrontendPackage? GetPackage(string name) => _microfrontends.FirstOrDefault(m => m.Name == name);

    public async Task DeletePackage(string name)
    {
        var packages = _microfrontends.Where(m => m.Name == name).ToList();

        if (packages.Any())
        {
            foreach (var package in packages)
            {
                package.PackageChanged -= NotifyPackagesChanged;
                _microfrontends.Remove(package);
                await package.Destroy();
                package.Dispose();
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
            package.PackageChanged += NotifyPackagesChanged;
            await Update();
        }
    }

    public void Dispose()
    {
        _microfrontends.ForEach(package =>
        {
            package.PackageChanged -= NotifyPackagesChanged;
            package.Dispose();
        });
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
        NotifyPackagesChanged(this, EventArgs.Empty);
    }

    private void NotifyPackagesChanged(object? sender, EventArgs e) => PackagesChanged?.Invoke(sender, e);
}
