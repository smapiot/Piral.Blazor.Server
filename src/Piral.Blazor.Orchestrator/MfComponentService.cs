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

    public IEnumerable<string> Scripts => ActivePackages.SelectMany(m => m.Scripts.Select(s => m.Service.GetLink(s)));

    public IEnumerable<string> Styles => ActivePackages.SelectMany(m => m.Styles.Select(s => m.Service.GetLink(s)));

    public IEnumerable<string> ComponentNames => ActivePackages.SelectMany(m => m.ComponentNames).Distinct();

    public IEnumerable<(string, string, Type)> Components => 
        ActivePackages.SelectMany(mf => mf.Components.Select((component) => (mf.Name, component.Name, component.Type)));

    public IEnumerable<(string, Type)> GetComponents(string name) =>
        ActivePackages.SelectMany(m => m.GetComponents(name).Select(component => (m.Name, component)));
}
