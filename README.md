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

## Extended Configuration

(tbd)

## Creating Micro Frontends

### Prerequisites

From scratch you can create a new Razor Component Library (RCL) project. By changing the csproj file's SDK to `Piral.Blazor.Sdk` you will be able to debug / develop this very conveniently.

The RCL has to be for .NET 8.

### Module Definition / Registration and Usage of Components

In order to be a valid micro frontend there has to be *one* **public** class that inherits from `IMfModule`:

```cs
public class Module : IMfModule
{
    public Module(IConfiguration configuration)
    {
        // Inject here what you want, e.g., the global `IConfiguration`.
    }

    public void Configure(IServiceCollection services)
    {
        // Configure your services in this function
    }

    public Task Setup(IMfAppService app)
    {
        // Register components and more
        return Task.CompletedTask;
    }

    public Task Teardown(IMfAppService app)
    {
        // Unregister things that need to be cleaned up
        return Task.CompletedTask;
    }
}
```

In the `Setup` function you can wire up your components to names that can be used on the outside. For instance, to wire up a `MapComponent` Razor component to an outside name of "mfa-map" you can do:

```cs
app.MapComponent<MapComponent>("mfa-map");
```

If you need to set up more things - such as scripts or stylesheets used by your dependencies you'd do:

```cs
app.AppendScript($"https://mycdn.com/some-global-script.js");
app.AppendScript("_content/BlazorGoogleMaps/js/objectManager.js");
```

The paths will be set up / configured correctly by the app shell.

### Dependencies

Just install your dependencies as you like; if they are correctly in the csproj they will be correctly in the NuGet package.

As an example, the following is a valid *csproj* file for a micro frontend:

```xml
<Project Sdk="Piral.Blazor.Sdk/0.1.0">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Version>0.4.0</Version>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AppShell>NNE.Emulator/0.1.0</AppShell>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BlazorGoogleMaps" Version="3.1.2" />
    <PackageReference Include="BlazorOcticons" Version="1.0.4" />
  </ItemGroup>

</Project>
```

It denotes two dependencies; `BlazorGoogleMaps` and `BlazorOcticons`. The resulting NuGet package will reference these two dependencies, too - and therefore these two will be part of the server's / app shell's resolution.

## Using Components from Micro Frontends

To use a component (such as "mfa-components" - this name is defined by the micro frontend calling the `MapComponent` method of the `IMfAppService` instance passed to their module definition - see below) without any parameters:

```razor
<MfComponent Name="mfa-component" />
```

You can also specify parameters if necessary / wanted:

```razor
<MfComponent Name="mfa-component" Parameters="@parameters" />
```

where

```cs
private Dictionary<string, object> parameters = new Dictionary<string, object>
{
    { "foo", 5 }
};
```

The `MfComponent` component is available in the `Piral.Blazor.Shared` NuGet package. It can be used in the server / app shell or in any micro frontend.

## License

Piral.Blazor is released using the MIT license. For more information see the [license file](https://raw.githubusercontent.com/smapiot/Piral.Blazor.Server/main/LICENSE).
