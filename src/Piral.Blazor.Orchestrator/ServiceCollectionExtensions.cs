using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Piral.Blazor.Orchestrator.Connector;
using Piral.Blazor.Orchestrator.Loader;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrofrontends<TLoader>(this IServiceCollection services, PiralOptions? options = default)
        where TLoader : class, IMfLoaderService
    {
        var config = new ProvidedPiralConfig(options ?? new PiralOptions());

        services.AddSingleton<IPiralConfig>(config);
        services.AddSingleton<IModuleContainerService, ModuleContainerService>();
        services.AddSingleton<IMfRepository, MfRepository>();
        services.AddSingleton<INugetService, NugetService>();
        services.AddSingleton<IMfPackageService, MfPackageService>();
        services.AddSingleton<IGlobalEvents, EventBus>();
        services.AddSingleton<IData, GlobalData>();
        services.AddScoped<IScopedEvents, EventBus>();

        if (config.IsEmulator)
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
