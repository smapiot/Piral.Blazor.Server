# Command Line Tooling

The dotnet tool (CLI) for working with Piral.Blazor.Server-based applications.

[[toc]]

## Installation

To install the tool globally run the following command:

```sh
dotnet tool install --global Piral.Blazor.Cli
```

This will install the `piral-blazor-server` tool in the standard binary directory. Now you should be able to use it already.

## Running

You can invoke the the tool by running the following command:

```sh
piral-blazor-server <command>
```

where `<command` is one of the following commands:

- `create-emulator`
- `prefill-cache`
- `publish-microfrontend`

The following sections go into details on these commands.

### Create Emulator

The command `piral-blazor-server create-emulator` can be used to create an emulator NuGet package for the current Piral.Blazor server (also referred to as app shell).

Example:

```sh
piral-blazor-server create-emulator -o dist
```

Creates an emulator (NuGet package) in the `dist` directory. The `csproj` file for the example above is assumed to be in the working directory.

### Prefill Cache

The command `piral-blazor-server prefill-cache` can be used to prefill the cache. This is useful when you want to operate on a static / pre-configured set of micro frontends. Also, it helps to improve the startup performance. Usually, this command would be used in a CI/CD pipeline, i.e., before the server is actually started or run somewhere.

Example:

```sh
piral-blazor-server prefill-cache --environment Production --source ./App --output ./App/.cache
```

Prefills the cache using the `appsettings.Production.json` and `appsettings.json` files from the `App` subdirectory of the current working directory. Puts the files in the `App/.cache` directory.

### Publish Micro Frontend

The command `piral-blazor-server publish-microfrontend` can be used to build and publish a micro frontend. This builds, packs, and publishes the micro frontend as a NuGet package.

Example:

```sh
piral-blazor-server publish-microfrontend --source ./SomeMf --url https://feed.piral.cloud/api/v1/nuget/myfeed --key abcdef1234
```

Builds, packs, and publishes the project found in the `SomeMf` directory of the current working directory. Uses the given URL as NuGet feed. The authentication is based on the provided key.
