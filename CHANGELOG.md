# Piral.Blazor.Server Changelog

## 0.5.0 (tbd)

- Fixed issue with broken assets
- Fixed emulator running debugged micro frontend assemblies in default context
- Updated Autoface to v9.0.0 (#6)
- Added `PiralOptions` to configure behavior
- Added `IsolatedAssemblies` option
- Added support for relative paths in `AppShell` of SDK (#2)
- Added new documentation page (blazor.piral.io)
- Added support for `IConfiguration` of micro frontends
- Added roadmap (#3)

## 0.4.1 (February 16, 2024)

- Fixed issue with `piral-blazor-server publish-microfrontend` to take wrong path
- Updated NuGet dependencies
- Updated documentation
- Added support for special fields (`Id`) in micro frontend discovery
- Added automatic detection of NuGet feed usage as discovery service

## 0.4.0 (February 14, 2024)

- Updated to .NET 8
- Updated documentation
- Fixed issue with parallelized loading of micro frontends
- Fixed issue when config section `Microfrontends:NugetFeeds` does not exist
- Fixed issue with the snapshot service requiring write access in cache-only
- Added `TryGetData` and `TrySetData` methods to `IMfService`
- Added `Meta` property to `IMfService`
- Added dotnet CLI tool `piral-blazor-server`

## 0.3.0 (November 15, 2023)

- Updated the API of the `IMfComponentService` interface to support more use cases
- Added middleware running in development providing enhanced debugging capabilities
- Added useful extensions for the `IMfComponentService` service
- Added `PageStyles` and `PageScripts` components to decorate layout
- Added `MfRouter` and `MfRouteView` components to enable routing (opt-in)
- Added integration to Piral Inspector protocol for enhanced development

## 0.2.1 (October 19, 2023)

- Fixed issue with nullables in SDK
- Fixed issue with scoped service provider usage
- Improved scoping and restart of page with scoped child providers

## 0.2.0 (October 17, 2023)

- First preview release
