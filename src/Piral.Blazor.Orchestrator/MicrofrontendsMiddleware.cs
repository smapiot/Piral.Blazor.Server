﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Piral.Blazor.Orchestrator;

internal class MicrofrontendsMiddleware
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private readonly RequestDelegate _next;
    private readonly IMfRepository _repository;

    public MicrofrontendsMiddleware(RequestDelegate next, IMfRepository repository)
    {
        _contentTypeProvider = new FileExtensionContentTypeProvider();
        _next = next;
        _repository = repository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/assets"))
        {
            var segments = context.Request.Path.Value?.Split('/') ?? Array.Empty<string>();

            if (segments.Length > 3)
            {
                // [0] -> empty ""
                // [1] -> "assets"
                var name = segments[2]; // [2] -> <mf-name>
                var path = string.Join('/', segments.Skip(3));
                var package = _repository.Packages.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (package is not null)
                {
                    using var ms = package.GetFile(path);

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

        await _next(context);
    }
}