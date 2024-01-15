using Microsoft.AspNetCore.Components;
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
        services.AddSingleton<IComponentActivator, MfComponentActivator>();
        services.AddSingleton<IMfRepository, MfRepository>();
        services.AddSingleton<INugetService, NugetService>();
        services.AddSingleton<IMfPackageService, MfPackageService>();
        services.AddSingleton<IEvents, GlobalEvents>();

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
}
