using Microsoft.AspNetCore.Builder;

namespace Piral.Blazor.Orchestrator;

public static class ApplicationBuilderExtensions
{

    public static IApplicationBuilder UseMicrofrontends(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MicrofrontendsMiddleware>();
    }
}
