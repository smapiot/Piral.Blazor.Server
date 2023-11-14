using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public class MfRouter : Router
{
    private IMfComponentService? _componentService;

    [Inject]
    public IMfComponentService? ComponentService
    {
        get { return _componentService; }
        set
        {
            _componentService = value;
            AdditionalAssemblies = GetAssemblies();
        }
    }

    private IEnumerable<Assembly> GetAssemblies()
    {
        if (_componentService is not null)
        {
            return _componentService.GetAllRouteComponents().Select(m => m.Component.Assembly).Distinct();
        }

        return Enumerable.Empty<Assembly>();
    }
}