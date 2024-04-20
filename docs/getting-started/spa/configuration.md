# Build Configuration

The main build configuration is done via the project file. Additionally, some special files are considered, too.

[[toc]]

## Project File

The `*.csproj` file of your pilet offers you some configuration steps to actually tailor the build to your needs.

Here is a minimal example configuration:

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <PiralInstance>../../app-shell/dist/emulator/app-shell-1.0.0.tgz</PiralInstance>
  </PropertyGroup>

  <!-- ... -->
</Project>
```

This one gets the app shell from a local directory. Realistically, you'd have your app shell on a registry. In case of the default registry it could look like

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <PiralInstance>@mycompany/app-shell</PiralInstance>
  </PropertyGroup>

  <!-- ... -->
</Project>
```

but realistically you'd publish the app shell to a private registry on a different URL. In such scenarios you'd also make use of the `NpmRegistry` setting:

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <PiralInstance>@mycompany/app-shell</PiralInstance>
    <NpmRegistry>https://registry.mycompany.com/</NpmRegistry>
  </PropertyGroup>

  <!-- ... -->
</Project>
```

Besides these two options (required `PiralInstance` and optional `NpmRegistry`) the following settings exist:

- `Version`: Sets the version of the pilet. This is a/the standard project property.
- `PiralInstance`: Sets the name (or local path) of the app shell.
- `NpmRegistry`: Sets the URL of the npm registry to use. Will be used for getting npm dependencies of the app shell (and the app shell itself).
- `Bundler`: Sets the name of the bundler to use. By default this is `esbuild`. The list of all available bundlers can be found [in the Piral documentation](https://docs.piral.io/reference/documentation/bundlers).
- `ProjectsWithStaticFiles`: Sets the names of the projects that contain static files, which require to be copied to the output directory. Separate the names of these projects by semicolons.
- `Monorepo`: Sets the behavior of the scaffolding to a monorepo mode. The value must be `enable` to switch this on.
- `PiralCliVersion`: Determines the version of the `piral-cli` tooling to use. By default this is `latest`.
- `PiralBundlerVersion`: Determines the version of the `piral-cli-<bundler>` to use. By default, this is the same as the value of the `PiralCliVersion`.
- `OutputFolder`: Sets the temporary output folder for the generated pilet (default: `..\piral~`).
- `ConfigFolder`: Sets the folder where the config files are stored (default: *empty*, i.e., current project folder).
- `MocksFolder`: Sets the folder where the Kras mock files are stored (default: `.\mocks`).
- `PiletKind`: Sets the pilet kind (values: `global`, `local`; default: `local`).
- `PiletPriority`: Sets the optional priority of the pilet when loading (any representable positive number). DLLs of Blazor pilets with higher numbers will *always* be loaded before the current DLLs (default: *none*).
- `PublishFeedUrl`: Sets the URL to be used for publishing the pilet. If this is left free then using "Publish" in Visual Studio will not trigger a publish of the pilet.
- `PublishFeedApiKey`: Sets the API Key to be used when publishing the pilet. If this is left free then the interactive upload is used, which will open a web browser for logging into the feed service.

A more extensive example:

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Version>1.2.3</Version>
    <PiralInstance>@mycompany/app-shell</PiralInstance>
    <PiralCliVersion>next</PiralCliVersion>
    <PiralBundlerVersion>1.1.0</PiralBundlerVersion>
    <NpmRegistry>https://registry.mycompany.com/</NpmRegistry>
    <Bundler>esbuild</Bundler>
    <Monorepo>disable</Monorepo>
    <ProjectsWithStaticFiles>
      designsystem;
      someotherproject;
      thirdproj
    </ProjectsWithStaticFiles>
    <PiletPriority>999</PiletPriority>
  </PropertyGroup>

  <!-- ... -->
</Project>
```

While pilets that define `PiletKind` to be `global` only have *shared dependencies*, the default for `local` pilets is to have *integrated dependencies*. If certain dependencies of `local` pilets should also be loaded into the global context (effectively sharing the dependency between all pilets - independent of the version) then you need to put those dependencies into a dedicated `ItemGroup` using the `Label` `shared`:

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <!-- ... -->

  <ItemGroup Label="shared">
    <PackageReference Include="Autofac.Extensions.DependencyInjection" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- ... -->
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="8.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="8.0.1" PrivateAssets="all" />
  </ItemGroup>

</Project>
```

## Special Files

There are some special files that you can add in your project (adjacent to the *.csproj* file):

- *setup.tsx*
- *teardown.tsx*
- *package-overwrites.json*
- *meta-overwrites.json*
- *kras-overwrites.json*
- *js-imports.json*

**Note**: The location of these files can also be changed through the `ConfigFolder` option. By default, this one is empty, i.e., all files have to be placed adjacent to the *.csproj* file as mentioned above. However, if you set the value to, e.g., *.piletconfig* then the files will be retrieved from this subdirectory. For instance, the setup file would then be read from *.piletconfig/setup.tsx*.

Let's see what these files do and how they can be used.

### Extending the Pilet's Setup

The *setup.tsx* file can be used to define more things that should be done in a pilet's `setup` function. By default, the content of the `setup` function is auto generated. Things such as `@page /path-to-use` components or components with `@attribute [PiralExtension("name-of-slot")]` would be automatically registered. However, already in case of `@attribute [PiralComponent]` we have a problem. What should this component do? Where is it used?

The solution is to use the *setup.tsx* file. An example:

```js
export default (app) => {
  app.registerMenu(app.fromBlazor('counter-menu'));

  app.registerExtension("ListToggle", app.fromBlazor('counter-preview'));
};
```

This example registers a pilet's component named "counter-menu" as a menu entry. Furthermore, it also adds the "counter-preview" component as an extension to the "ListToggle" slot.

Anything that is available on the Pilet API provided via the `app` argument is available in the function. The only import part of *setup.tsx* is that has a default export - which is actually a function.

### Overwriting the Package Manifest

The generated / used pilet is a standard npm package. Therefore, it will have a *package.json*. The content of this *package.json* is mostly pre-determined. Things such as `piral-cli` or the pilet's app shell package are in there. In some cases, additional JS dependencies for runtime or development aspects are necessary or useful. In such cases the *package-overwrites.json* comes in handy.

For instance, to actually extend the `devDependencies` you could write:

```json
{
  "devDependencies": {
    "axios": "^0.20.0"
  }
}
```

This would add a development dependency to the `axios` package. Likewise, other details, such as a publish config or a description could also be added / overwritten:

```json
{
  "description": "This is my pilet description.",
  "publishConfig": {
    "access": "public"
  }
}
```

The rules for the merge follow the [Json.NET](https://www.newtonsoft.com/json/help/html/MergeJson.htm) approach.

### Overwriting the Debug Meta Data

The generated / used pilet is served from the local file system instead of a feed service. Therefore, it will not have things like a configuration store. However, you might want to use one - or at least test against one. For this, usually a *meta.json* file can be used. The content of this *meta.json* is then merged into the metadata of a served pilet. For Piral.Blazor this file is premade, however, its content can still be overwritten using a *meta-overwrites.json* file.

For instance, to include a custom `config` field (with one config called `backendUrl`) in the pilet's metadata you can use the following content:

```json
{
  "config": {
    "backendUrl": "http://localhost:7345"
  }
}
```

The rules for the merge follow the [Json.NET](https://www.newtonsoft.com/json/help/html/MergeJson.htm) approach.

### Extending the Pilet's Teardown

The *teardown.tsx* file can be used to define more things that should be done in a pilet's `teardown` function. By default, the content of the `teardown` function is auto generated. Things such as `pages` and `extensions` would be automatically unregistered. However, in some cases you will need to unregister things manually. You can do this here.

### Defining Additional JavaScript Imports

Some Blazor dependencies require additional JavaScript packages in order to work correctly. The *js-imports.json* file can be to declare all these. The files will then be added via a generated `import` statement in the pilet's root module.

The content of the *js-imports.json* file is a JSON array. For example:

```json
[
  "axios",
  "global-date-functions"
]
```

Includes the two dependencies via the respective `import` statements.

### DevServer Settings

The `Piral.Blazor.DevServer` can be configured, too. Much like the standard / official Blazor DevServer you can introduce a *blazor-devserversettings.json* file that describes more options. Most of the contained options are the same as the one for the official Blazor DevServer.

Current options found in the `Piral` section:

- `forwardedPaths` - is an array of strings describing the path segments that should be forwarded to the Piral CLI dev server (using kras)

  Example:

  ```json
  {
    "Piral": {
      "forwardedPaths": [ "/foo" ]
    }
  }
  ```

- `feedUrl` - is a string defining an URL for including an external / remote feed of pilets into the debug process

  Example:

  ```json
  {
    "Piral": {
      "feedUrl": "https://feed.piral.cloud/api/v1/pilet/sample"
    }
  }
  ```

In addition, the options for the DevServer also touch the configured options for the `Piral.Blazor.Tools`, such as `OutputFolder` which is used to define where the scaffolded pilet is stored.

### Setting the Logging Level

The log level can be set either within your Blazor pilets using the `ILoggingConfiguration` service or from JavaScript:

```js
DotNet.invokeMethodAsync('Piral.Blazor.Core', 'SetLogLevel', logLevel);
```

Here, the value for `logLevel` should be between 0-6, where 0 logs everything (even traces) and 6 logs nothing. Alternatively, you can also set a log level when initializing `piral-blazor`.
