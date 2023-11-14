using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public static class MfComponentServiceExtensions
{
    private const string ROUTE_PREFIX = "route:";

    public static IEnumerable<string> GetRoutes(this IMfComponentService service)
    {
        foreach (var name in service.ComponentNames)
        {
            if (name.StartsWith(ROUTE_PREFIX))
            {
                var route = name[ROUTE_PREFIX.Length..];
                yield return route;
            }
        }
    }

    public static IEnumerable<(string Route, string Microfrontend, Type Component)> GetAllRouteComponents(this IMfComponentService service)
    {
        foreach (var name in service.ComponentNames)
        {
            if (name.StartsWith(ROUTE_PREFIX))
            {
                var route = name[ROUTE_PREFIX.Length..];
                var components = service.GetComponents(name);

                foreach (var (microfrontend, type) in components)
                {
                    yield return (route, microfrontend, type);
                }
            }
        }
    }

    public static IEnumerable<(string Microfrontend, Type Component)> GetRouteComponents(this IMfComponentService service, string routeTemplate)
    {
        var componentName = $"{ROUTE_PREFIX}{routeTemplate}";
        return service.GetComponents(componentName);
    }

    public static string? GetComponentOrigin(this IMfComponentService service, Type component)
    {
        return service.Components.Where(m => m.Component == component).Select(m => m.Microfrontend).FirstOrDefault();
    }
}
