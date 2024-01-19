[![Piral Logo](https://github.com/smapiot/piral/raw/main/docs/assets/logo.png)](https://piral.io)

# Piral.Blazor.Orchestrator &middot; [![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/smapiot/Piral.Blazor.Server/blob/main/LICENSE) | [![GitHub Tag](https://img.shields.io/github/tag/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/releases) [![GitHub Issues](https://img.shields.io/github/issues/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Server.Blazor/issues) [![Gitter Chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://matrix.to/#/#piral-io_blazor:gitter.im)

> The orchestration module for creating server-side micro frontends using Blazor.

## Installation & Setup

For using `Piral.Blazor.Server` you'll need an ASP.NET Core project using Blazor (server).

You'll only need to add a single NuGet package to the project:

```ps1
install-package Piral.Blazor.Orchestrator
```

With the package installed you'll need to configure your project to actually use `Piral.Blazor.Orchestrator`.

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

If you want to enable routing for your micro frontends (such that they can use the `MapRoute` feature) you should also exchange the `Router` in your `App.razor` with the `MfRouter` like so:

```razor
<MfRouter AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <MfRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</MfRouter>
```

The rest you can keep (or change) as you like.

**Note**: Using the `MfRouteView` in the code above is *optional*. We do recommend it, however, if you just keep on using `RouteView` then it would work, too.

Finally, remove the reference to any `blazor.server.js` script, i.e., transform your `_host.cshtml` to have no `<script>` tag such as:

```html
@page "/"
@using Microsoft.AspNetCore.Components.Web
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="~/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link href="css/site.css" rel="stylesheet" />
    <link href="nne.server.app.styles.css" rel="stylesheet" />
    <link rel="icon" type="image/png" href="favicon.png"/>
    <component type="typeof(HeadOutlet)" render-mode="ServerPrerendered" />
</head>
<body>
    <component type="typeof(App)" render-mode="ServerPrerendered" />

    <div id="blazor-error-ui">
        <environment include="Development">
            An unhandled exception has occurred. See browser dev tools for details.
        </environment>
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
</body>
</html>
```

The script will be injected (and run) from the orchestrator.
