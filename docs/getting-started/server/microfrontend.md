# Micro Frontend Setup

In general, to create a Blazor pilet using `Piral.Blazor.Server` you should just create a Razor Component Library (RCL) project. This way, you will need to make the least changes.

[[toc]]

## Prerequisites

You will need to have an app shell using `Piral.Blazor.Orchestrator` available somewhere.

From scratch you can create a new Razor Component Library (RCL) project. By changing the csproj file's SDK to `Piral.Blazor.Sdk` you will be able to debug / develop this very conveniently.

The RCL has to be for .NET 8.

## Preparation

You will need to leverage the `Piral.Blazor.Sdk` SDK in the csproj file like this:

```xml
<Project Sdk="Piral.Blazor.Sdk/0.5.0">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Version>1.0.0</Version>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AppShell>My.Emulator/0.1.0</AppShell>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="My.Emulator" Version="0.1.0" PrivateAssets="all" />
    <PackageReference Include="BlazorGoogleMaps" Version="3.1.2" />
    <PackageReference Include="BlazorOcticons" Version="1.0.4" />
  </ItemGroup>

</Project>
```

The example shows a micro frontend using an app shell deployed as `My.Emulator` in version `1.0.0`. The micro frontend brings its own dependencies, namely `BlazorGoogleMaps` and `BlazorOcticons`.

### Referencing the Emulator

There are multiple ways of how to reference the emulator:

1. A globally installed NuGet package
2. A locally installed NuGet package
3. An absolute file path
4. A relative file path

Let's go over these possibilities quickly. Arguably, by far the most common is (2), as it works reliably independent of current machine's configuration.

#### Globally Installed NuGet Package

For this the emulator needs already to be installed on your system.

```xml
<Project Sdk="Piral.Blazor.Sdk/0.5.0">
  <PropertyGroup>
    <AppShell>My.Emulator/0.1.0</AppShell>
  </PropertyGroup>
</Project>
```

#### Locally Installed NuGet Package

By placing the emulator in a `PackageReference` with `PrivateAssets` set to `all` you'll make sure that NuGet downloads the package - properly populating the package cache.

```xml
<Project Sdk="Piral.Blazor.Sdk/0.5.0">
  <PropertyGroup>
    <AppShell>My.Emulator/0.1.0</AppShell>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="My.Emulator" Version="0.1.0" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

#### Absolute File Path

The full path leading to the artifacts produced by the emulator project.

```xml
<Project Sdk="Piral.Blazor.Sdk/0.5.0">
  <PropertyGroup>
    <AppShell>C:\Code\My.Emulator\bin\Debug\net8.0\publish</AppShell>
  </PropertyGroup>
</Project>
```

#### Relative File Path

The relative path leading to the artifacts produced by the emulator project. Uusally, you want to be relative from the `csproj` location of the current project.

```xml
<Project Sdk="Piral.Blazor.Sdk/0.5.0">
  <PropertyGroup>
    <AppShell>../../My.Emulator/bin/Debug/net8.0/publish</AppShell>
  </PropertyGroup>
</Project>
```

## Module Definition / Registration and Usage of Components

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

## Dependencies

Just install your dependencies as you like; if they are correctly in the csproj they will be correctly in the NuGet package.

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
