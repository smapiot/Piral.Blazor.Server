[![Piral Logo](https://github.com/smapiot/piral/raw/main/docs/assets/logo.png)](https://piral.io)

# Piral.Blazor.Cli &middot; [![GitHub License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/smapiot/Piral.Blazor.Server/blob/main/LICENSE) | [![GitHub Tag](https://img.shields.io/github/tag/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Blazor.Server/releases) [![GitHub Issues](https://img.shields.io/github/issues/smapiot/Piral.Blazor.Server.svg)](https://github.com/smapiot/Piral.Server.Blazor/issues) [![Gitter Chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://matrix.to/#/#piral-io_blazor:gitter.im)

> The dotnet tool (CLI) for working with Piral.Blazor.Server-based applications.

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

The following sections go into details on these commands.
