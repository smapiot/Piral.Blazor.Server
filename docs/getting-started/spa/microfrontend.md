# Creating a Blazor Pilet

We recommend that you watch the video [on scaffolding from the standard VS template](https://youtu.be/Ychzp2xMxes) before you go over the details below.

In general, to create a Blazor pilet using `Piral.Blazor`, two approaches can be used:

[[toc]]

## From Scratch

In this case, it is highly recommended to use our template. More information and installation instructions can be found in [`Piral.Blazor.Template`](https://www.nuget.org/packages/Piral.Blazor.Template).

## From an Existing Application

In this case, follow these steps:

1. Add a `PiralInstance` property to your `.csproj` file (The Piral instance name should be the name of the Piral instance you want to use, as it is published on npm.)

   ```xml
   <PropertyGroup>
       <TargetFramework>net8.0</TargetFramework>
       <PiralInstance>my-piral-instance</PiralInstance>
   </PropertyGroup>
   ```

   (You can optionally also specify an `NpmRegistry` property. The default for this is set to `https://registry.npmjs.org/`)

2. Install the `Piral.Blazor.Tools` and `Piral.Blazor.Utils` packages, make sure they both have a version number of format `8.0.x`
3. Remove the `Microsoft.AspNetCore.Components.WebAssembly.DevServer` package and install the `Piral.Blazor.DevServer` package (using the same version as the packages from (2))
4. Rename `Program.cs` to `Module.cs`, and make sure to make the `Main` method an empty method.
5. Build the project. The first time you do this, this can take some time as it will fully scaffold the pilet.

If you run the solution using `F5` the `Piral.Blazor.DevServer` will start the Piral CLI under the hood. This allows you to not only use .NET Hot-Reload, but also replace the pilets on demand.

## Global vs Local Pilet

By default, a pilet is treated as an isolated assembly. This makes sense, as it potentially was also developed in isolation and should not conflict with another micro frontend. Nevertheless, there are cases where you potentially want to break this isolation, e.g.:

- When providing some services that can be used from other pilets
- When manipulating global settings or configurations
- When sharing dependencies that other pilets should not have to load on their own

For these cases (and others) another type of pilet has been introduced: *global* pilets.

While *local* pilets will run in their own context, loading their own dependencies, and specifying their own services, *global* pilets are run in the default context - sharing their dependencies with the rest of the application and introducing services for all pilets.

Global pilets are also built without optimizations - this is done to prevent issues that might occur when shared dependencies are trimmed and therefore no longer working as anticipated by other pilets.
