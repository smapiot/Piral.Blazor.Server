using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Piral.Blazor.Orchestrator.Loader;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMicrofrontends<TLoader>(this IServiceCollection services)
		where TLoader : class, IMfLoaderService
	{
		services.AddSingleton<IModuleContainerService, ModuleContainerService>();
		services.AddSingleton<IComponentActivator, MfComponentActivator>();
		services.AddSingleton<IMfRepository, MfRepository>();
		services.AddSingleton<INugetService, NugetService>();
        services.AddSingleton<IMfPackageService, MfPackageService>();
		services.AddSingleton<IEvents, GlobalEvents>();

		if (Environment.GetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY") is not null)
        {
            services.AddSingleton<TLoader>();
            services.AddSingleton<IMfLoaderService, MfLocalLoaderService<TLoader>>();
        }
		else
        {
            services.AddSingleton<IMfLoaderService, TLoader>();
        }

		services.AddSingleton<ISnapshotService, FsNugetSnapshotService>();
        services.AddSingleton<IMfComponentService, MfComponentService>();
        services.AddHostedService<MfOrchestrationService>();
		return services;
	}
}
