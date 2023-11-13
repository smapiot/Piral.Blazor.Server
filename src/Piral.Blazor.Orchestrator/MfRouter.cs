using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public class MfRouter : Router
{
    private const string ROUTE_PREFIX = "route:";
    private IMfComponentService? _componentService;

    [Inject]
    public IMfComponentService? ComponentService
    {
        get { return _componentService; }
        set
        {
            _componentService = value;
            AdditionalAssemblies = GetAssemblies().Distinct();
        }
    }

    private IEnumerable<Assembly> GetAssemblies()
    {
        if (_componentService is not null)
        {
            foreach (var name in _componentService.ComponentNames)
            {
                if (name.StartsWith(ROUTE_PREFIX))
                {
                    var components = _componentService.GetComponents(name);

                    foreach (var (_, component) in components)
                    {
                        yield return component.Assembly;
                    }
                }
            }
        }
    }
}