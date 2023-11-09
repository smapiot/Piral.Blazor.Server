using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

internal class MfComponentService : IMfComponentService
{
    private readonly IMfRepository _repository;

    public event EventHandler? ComponentsChanged;

    public MfComponentService(IMfRepository repository)
    {
        _repository = repository;
        _repository.PackagesChanged += OnPackagesChanged;
    }

    private void OnPackagesChanged(object? sender, EventArgs e) => ComponentsChanged?.Invoke(this, e);

    private IEnumerable<MicrofrontendPackage> ActivePackages => _repository.Packages.Where(m => !m.IsDisabled);

    public IEnumerable<string> Scripts => ActivePackages.SelectMany(m => m.Scripts.Select(s => GetLink(m.Name, s)));

    public IEnumerable<string> Styles => ActivePackages.SelectMany(m => m.Styles.Select(s => GetLink(m.Name, s)));

    public IEnumerable<string> ComponentNames => ActivePackages.SelectMany(m => m.ComponentNames).Distinct();

    public IEnumerable<(string, Type)> GetComponents(string name) =>
        ActivePackages.SelectMany(m => m.GetComponents(name).Select(component => (m.Name, component)));

    private static string GetLink(string name, string path)
    {
        if (path.StartsWith("http:") || path.StartsWith("https:"))
        {
            return path;
        }
        else if (path.StartsWith("/"))
        {
            return GetLink(name, path.Substring(1));
        }
        else
        {
            return $"/assets/{name}/{path}";
        }
    }

}
