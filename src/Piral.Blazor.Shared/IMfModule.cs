using Microsoft.Extensions.DependencyInjection;

namespace Piral.Blazor.Shared;

public interface IMfModule
{
    Task Setup(IMfAppService app);

    Task Teardown(IMfAppService app);

    void Configure(IServiceCollection services);
}
