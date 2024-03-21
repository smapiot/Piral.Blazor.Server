using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Piral.Blazor.Orchestrator;

internal class MicrofrontendsMiddleware(RequestDelegate next, IMfRepository repository, IMfDebugConnector debug)
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();
    private readonly RequestDelegate _next = next;
    private readonly IMfRepository _repository = repository;
    private readonly IMfDebugConnector _debug = debug;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/assets"))
        {
            var segments = context.Request.Path.Value?.Split('/') ?? [];

            if (segments.Length > 3)
            {
                // [0] -> empty ""
                // [1] -> "assets"
                var name = segments[2]; // [2] -> <mf-name>
                var path = string.Join('/', segments.Skip(3));
                var package = _repository.Packages.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (package is not null)
                {
                    using var ms = await package.GetFile(path);

                    if (ms is not null)
                    {
                        _contentTypeProvider.TryGetContentType(path, out var contentType);
                        context.Response.ContentType = contentType ?? "application/octet-stream";
                        context.Response.StatusCode = 200;
                        await ms.CopyToAsync(context.Response.Body);
                        return;
                    }
                }
            }
        }
        else if (await _debug.InterceptAsync(context))
        {
            return;
        }

        await _next(context);
    }
}