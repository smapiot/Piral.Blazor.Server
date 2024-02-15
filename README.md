[![Piral Logo](https://github.com/smapiot/piral/raw/main/docs/assets/logo.png)](https://piral.io)

# Piral.Blazor.Server &middot; [![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/smapiot/piral.blazor/blob/main/LICENSE) [![GitHub Tag](https://img.shields.io/github/tag/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/releases) [![GitHub Issues](https://img.shields.io/github/issues/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/issues) [![Gitter Chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/piral-io/blazor)

A dedicated and fully functional server-side composed micro frontend solution for Blazor.

## Documentation Page

We have a dedicated documentation page for all things related Blazor. In case you need more extensive information and a bit of background you should check this one out.

👉 [blazor.piral.io](https://blazor.piral.io)

👇 In this README you'll find the most important information to get started.

## Installation & Setup

For using `Piral.Blazor.Orchestrator` you'll need an ASP.NET Core project using Blazor (server).

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

More details on this [in the NuGet package README](./src/Piral.Blazor.Orchestrator/README.md).

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
    <TargetFramework>net8.0</TargetFramework>
    <Version>0.4.0</Version>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AppShell>My.Emulator/0.1.0</AppShell>
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
  { "Foo", 5 }
};
```

The `MfComponent` component is available in the `Piral.Blazor.Shared` NuGet package. It can be used in the server / app shell or in any micro frontend.

Alternatively, you can also specify parameters directly, e.g., for the previous example you could also write:

```razor
<MfComponent Name="mfa-component" Foo="5" />
```

## A Practical Example

A full example is online on GitHub:

👨‍💻 [FlorianRappl/Piral.Blazor.Server.Samples.Tractor](https://github.com/FlorianRappl/Piral.Blazor.Server.Samples.Tractor)

## License

Piral.Blazor is released using the MIT license. For more information see the [license file](https://raw.githubusercontent.com/smapiot/Piral.Blazor.Server/main/LICENSE).
