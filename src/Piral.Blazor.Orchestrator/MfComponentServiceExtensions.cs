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

    public static IEnumerable<(string, Type)> GetAllRouteComponents(this IMfComponentService service)
    {
        foreach (var name in service.ComponentNames)
        {
            if (name.StartsWith(ROUTE_PREFIX))
            {
                var components = service.GetComponents(name);

                foreach (var parts in components)
                {
                    yield return parts;
                }
            }
        }
    }

    public static IEnumerable<(string, Type)> GetRouteComponents(this IMfComponentService service, string routeTemplate)
    {
        var componentName = $"{ROUTE_PREFIX}{routeTemplate}";
        return service.GetComponents(componentName);
    }
}
