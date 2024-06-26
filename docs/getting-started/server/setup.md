# Server Setup

For the server you'll need to build a proper app shell first. If you already have one and need to get started with a micro frontend look at the next chapter.

[[toc]]

## Preparation

For using `Piral.Blazor.Server` you'll need an ASP.NET Core project using Blazor (server).

You'll only need to add a single NuGet package to the project:

```ps1
install-package Piral.Blazor.Orchestrator
```

With the package installed you'll need to configure your project to actually use `Piral.Blazor.Orchestrator`.

```cs
// Important - an `HttpClient` needs to be present for the MfDiscoveryLoaderService - for
// other services it might not be needed; so you can regard this as optional
builder.Services.AddHttpClient();
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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMicrofrontends<MfDiscoveryLoaderService>();

builder.Host.UseMicrofrontendContainers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseMicrofrontends();

app.MapMicrofrontends<App>();

app.Run();
```

## Layout Modification

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

::: info
If you skip `<PageScripts />` then Blazor will essentially run in an SSR-only mode. There will be no interactivity. Make sure to include this in your layout. Likewise, the `<PageStyles />` are quite important.
:::

## Routing

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

Finally, remove the reference to any Blazor script, i.e., transform your `App.razor` to have no `<script>` tag such as:

```html
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <link rel="stylesheet" href="app.css" />
    <HeadOutlet />
</head>

<body>
    <Routes @rendermode="InteractiveServer" />
</body>
</html>
```

The script will be injected (and run) from the orchestrator.

## Extended Configuration

By default, the micro frontend loader takes an empty feed of micro frontends. This way, nothing will be loaded and the application will remain empty with respect to the loaded micro frontends. To change this you will need to adjust the configuration, e.g., by modifying the `appsettings.json` file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Microfrontends": {
    "CacheDir": ".cache",
    "DiscoveryInfoUrl": "https://feed.piral.cloud/api/v1/microfrontends/myfeed",
    "DiscoveryUpdateUrl": "wss://feed.piral.cloud/api/v1/pilet/myfeed",
    "NugetFeeds": {
      "Source": {
        "Url": "https://feed.piral.cloud/api/v1/nuget/myfeed/index.json"
      },
      "Public": {
        "Url": "https://api.nuget.org/v3/index.json"
      }
    }
  }
}
```

The given configuration is an example. The `Microfrontends` section is used to include the respective configuration. If this configuration is not present then default values (which match the shown values exactly) are applied. If you, e.g., want to have your cache stored in a different (relative or absolute) directory you'd need to have at least a property `CacheDir` in the config.

The `NugetFeeds` need to have all the feeds you want to use for resolving the dependencies of your micro frontends. Keep in mind that the actual *nupkg* file containing the micro frontends also needs to be hosted somewhere. This feed, which is usually referenced from the feed service, therefore also needs to be part of this configuration.

An example with more NuGet feeds:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Microfrontends": {
    "CacheDir": ".cache",
    "DiscoveryInfoUrl": "https://feed.piral.cloud/api/v1/microfrontends/myfeed",
    "DiscoveryUpdateUrl": "wss://feed.piral.cloud/api/v1/pilet/myfeed",
    "NugetFeeds": {
      "Source": {
        "Url": "https://feed.piral.cloud/api/v1/nuget/myfeed/index.json"
      },
      "Public": {
        "Url": "https://api.nuget.org/v3/index.json"
      },
      "GitHub": {
        "Url": "https://nuget.pkg.github.com/MyUserName/index.json",
        "User": "",
        "Token": ""
      },
      "Telerik": {
        "Url": "https://nuget.telerik.com/v3/index.json",
        "User": "",
        "Token": ""
      }
    }
  }
}
```

As you can see the `User` and `Token` fields, which are usually required for authenticating against a private feed, are left empty. This is not a mistake, but actually a best practice. You can use the .NET secret manager to then fill these parts. More details can be found [in the Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux).

## Initial Options

The behavior of the library can be configured when the DI services are configured, e.g.:

```cs
builder.Services.AddMicrofrontends<MfDiscoveryLoaderService>(new()
{
    IsolatedAssemblies = ["BlazorOcticons.dll"],
});
```

This will instruct the Piral orchestrator to keep loading the locally available (yet centrally provided) `BlazorOctions.dll`.
