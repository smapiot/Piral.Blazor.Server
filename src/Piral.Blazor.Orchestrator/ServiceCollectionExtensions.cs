using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Piral.Blazor.Orchestrator.Connector;
using Piral.Blazor.Orchestrator.Loader;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrofrontends<TLoader>(this IServiceCollection services)
        where TLoader : class, IMfLoaderService
    {
        var isEmulator = Environment.GetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY") is not null;

        services.AddSingleton<IModuleContainerService, ModuleContainerService>();
        services.AddSingleton<ICacheManipulatorService, CacheManipulatorService>();
        services.AddSingleton<IMfRepository, MfRepository>();
        services.AddSingleton<IComponentActivator, MfComponentActivator>();
        services.AddSingleton<INugetService, NugetService>();
        services.AddSingleton<IMfPackageService, MfPackageService>();
        services.AddSingleton<IEvents, GlobalEvents>();
        services.AddSingleton<IData, GlobalData>();

        if (isEmulator)
        {
            services.AddSingleton<TLoader>();
            services.TryAddScoped<IMfDebugConnector, MfEmulatorConnector>();
            services.AddSingleton<IMfLoaderService, MfLocalLoaderService<TLoader>>();
        }
        else
        {
            services.TryAddScoped<IMfDebugConnector, MfEmptyConnector>();
            services.AddSingleton<IMfLoaderService, TLoader>();
        }

        services.TryAddSingleton<ISnapshotService, FsNugetSnapshotService>();
        services.AddSingleton<IMfComponentService, MfComponentService>();
        services.AddHostedService<MfOrchestrationService>();
        return services;
    }

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
