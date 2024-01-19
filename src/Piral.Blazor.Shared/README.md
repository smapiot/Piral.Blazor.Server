[![Piral Logo](https://github.com/smapiot/piral/raw/main/docs/assets/logo.png)](https://piral.io)

# Piral.Blazor.Shared &middot; [![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/smapiot/Piral.Blazor.Server/blob/main/LICENSE) | [![GitHub Tag](https://img.shields.io/github/tag/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/releases) [![GitHub Issues](https://img.shields.io/github/issues/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Server.Blazor/issues) [![Gitter Chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://matrix.to/#/#piral-io_blazor:gitter.im)

> The shared module with common type definitions for micro frontends using Blazor.

## Installation & Setup

This is a shared library that will be automatically installed and used with `Piral.Blazor.Orchestrator` or in a micro frontend when you leverage the `Piral.Blazor.Sdk` SDK like this:

```xml
<Project Sdk="Piral.Blazor.Sdk/0.4.0">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Version>1.0.0</Version>
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

The example shows a micro frontend using an app shell deployed as `My.Emulator` in version `1.0.0`. The micro frontend brings its own dependencies, namely `BlazorGoogleMaps` and `BlazorOcticons`.
