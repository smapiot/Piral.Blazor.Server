[![Piral Logo](https://github.com/smapiot/piral/raw/main/docs/assets/logo.png)](https://piral.io)

# Piral.Blazor.Server &middot; [![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/smapiot/piral.blazor/blob/main/LICENSE) [![GitHub Tag](https://img.shields.io/github/tag/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/releases) [![GitHub Issues](https://img.shields.io/github/issues/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/issues) [![Gitter Chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/piral-io/blazor)

A dedicated and fully functional server-side composed micro frontend solution for Blazor.

## Installation & Setup

For using `Piral.Blazor.Server` you'll need an ASP.NET Core project using Blazor (server).

You'll only need to add a single NuGet package to the project:

```ps1
install-package Piral.Blazor.Server
```

With the package installed you'll need to configure your project to actually use `Piral.Blazor.Server`.

```cs
// Add DI services
builder.Services.AddMicrofrontends<MfDiscoveryLoaderService>();

// Configure container
builder.Host.UseMicrofrontendContainers();

// Use middleware
app.UseMicrofrontends();
```

A full example using these three integration points looks like:

```cs
using Piral.Blazor.Orchestrator;
using Piral.Blazor.Orchestrator.Loader;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMicrofrontends<MfDiscoveryLoaderService>();

builder.Host.UseMicrofrontendContainers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMicrofrontends();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

With these in place you can modify your layout to integrate the necessary parts.

```razor
@inherits LayoutComponentBase

<PageTitle>Example</PageTitle>
<PageStyles />

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<PageScripts />
```

## Extended Configuration

(tbd)

## Creating Micro Frontends

(tbd)

## License

Piral.Blazor is released using the MIT license. For more information see the [license file](https://raw.githubusercontent.com/smapiot/Piral.Blazor.Server/main/LICENSE).
