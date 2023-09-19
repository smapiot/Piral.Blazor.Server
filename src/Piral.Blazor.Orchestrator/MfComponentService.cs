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

    public IEnumerable<string> Scripts => _repository.Packages.SelectMany(m => m.Scripts.Select(s => GetLink(m.Name, s)));

    public IEnumerable<string> Styles => _repository.Packages.SelectMany(m => m.Styles.Select(s => GetLink(m.Name, s)));

    public IEnumerable<string> ComponentNames => _repository.Packages.SelectMany(m => m.ComponentNames).Distinct();

    public IEnumerable<Type> GetComponents(string name) => _repository.Packages.SelectMany(m => m.GetComponents(name));

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
