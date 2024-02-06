using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public static class EndpointRouteBuilderExtensions
{
    public static RazorComponentsEndpointConventionBuilder MapMicrofrontends<TRootComponent>(this IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapRazorComponents<TRootComponent>();
        var repository = endpoints.ServiceProvider.GetService<IMfRepository>() ?? throw new InvalidOperationException("You need to use 'AddMicrofrontends()' before you can 'MapMicrofrontends()'.");
        var addedAssemblies = new HashSet<Assembly>();

        repository.PackagesChanged += (_, _) =>
        {
            var assemblies = repository.Packages.GetRouteAssemblies().Where(m => !addedAssemblies.Contains(m)).ToArray();

            foreach (var assembly in assemblies)
            {
                addedAssemblies.Add(assembly);
            }

            builder.AddAdditionalAssemblies(assemblies);
        };

        return builder.AddInteractiveServerRenderMode();
    }
}
