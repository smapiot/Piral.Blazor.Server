using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Piral.Blazor.Orchestrator;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseMicrofrontendContainers(this IHostBuilder builder)
    {
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        return builder;
    }
}
