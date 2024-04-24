# Publishing the Emulator

For developing a micro frontend you'll need an emulator. An emulator is just a NuGet package containing a special build of the app shell that you've build for running your micro frontends.

[[toc]]

## Publishing using the `piral-server-server` CLI

The command `piral-blazor-server create-emulator` can be used to create an emulator NuGet package for the current Piral.Blazor server, i.e., the app shell that should be used.

Example:

```sh
piral-blazor-server create-emulator -o dist
```

This command creates an emulator NuGet package in the `dist` directory. The `csproj` file for the example above is assumed to be in the working directory. The NuGet package can then be published using `nuget push` or `dotnet nuget push`.

## Publishing using the `dotnet` CLI

If you want to have full control then using a custom sequence for creating and publishing the emulator might be what you are looking for. For this you'll need the `dotnet` and the `nuget` CLI. You will also need to create a custom `*.nuspec` file.

We start the sequence by producing a release build of your application:

```sh
dotnet publish -c Release
```

This will create the emulator's files in the publish directory (`bin/Release/net8.0/publish`).

Now create a NuGet config *inside* the `bin/Release/net8.0/publish` directory using the following properties:

- Name of your app shell (or how you want to call the emulator package)
- Version of your app shell
- Proper description, author etc. fields

The `files` should be set to `**/*`, i.e., take all files of the publish directory and place it in the NuGet package.

You need the `nuget` command line tooling to run `nuget pack` without any compilation. If you only have `dotnet` then this won't work (as `dotnet` will only run against a csproj, which the publish folder does not have).

Once the `*.nupkg` file is ready you can publish it to the NuGet feed or your choice.
