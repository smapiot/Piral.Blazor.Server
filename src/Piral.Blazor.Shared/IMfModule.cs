using Microsoft.Extensions.DependencyInjection;

namespace Piral.Blazor.Shared;

/// <summary>
/// Indicates the entry module for a micro frontend.
/// </summary>
public interface IMfModule
{
    /// <summary>
    /// The setup method that is called to wire up the micro frontend.
    /// </summary>
    /// <param name="app">The API to wire up the components.</param>
    Task Setup(IMfAppService app);

    /// <summary>
    /// The teardown method that is called when disposing the micro frontend.
    /// </summary>
    /// <param name="app">The API to tear down the components.</param>
    Task Teardown(IMfAppService app);

    /// <summary>
    /// Configures the local DI of the micro frontend.
    /// </summary>
    /// <param name="services">The service collection to use.</param>
    void Configure(IServiceCollection services);
}
