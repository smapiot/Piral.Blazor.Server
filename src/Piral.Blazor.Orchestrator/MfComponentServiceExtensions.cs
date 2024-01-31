using Piral.Blazor.Shared;
using System.Reflection;

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

    public static IEnumerable<Assembly> GetRouteAssemblies(this IEnumerable<MicrofrontendPackage> packages)
    {
        var otherAssemblies = packages.SelectMany(m => m.Components.Where(m => m.Name.StartsWith(ROUTE_PREFIX)).Select(m => m.Type.Assembly)).Distinct();
        var isEmulator = Environment.GetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY") is not null;

        if (isEmulator)
        {
            var currentAssembly = typeof(ExtensionCatalogue).Assembly;
            return otherAssemblies.Concat(Enumerable.Repeat(currentAssembly, 1));
        }

        return otherAssemblies;
    }

    public static IEnumerable<(string Route, string Microfrontend, Type Component)> GetAllRouteComponents(this IMfComponentService service)
    {
        var isEmulator = Environment.GetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY") is not null;

        if (isEmulator)
        {
            yield return ("/$debug-extension-catalogue", "root", typeof(ExtensionCatalogue));
        }

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
