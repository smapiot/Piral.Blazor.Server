# Publishing a Micro Frontend

Micro frontends should be published to a discovery service as well as in form of a NuGet package. If you use the [Piral Cloud Feed Service](https://www.piral.cloud/) you can also just publish them as NuGet package; the discovery service will do the rest for you.

## Publishing using the `piral-server-server` CLI

First, make sure you have the CLI installed. If not, do so using this command:

```sh
dotnet tool install --global Piral.Blazor.Cli
```

This will install the piral-blazor-server tool in the standard binary directory. Now you should be able to use it already.

Now, you can use the `publish-microfrontend` command:

```sh
piral-blazor-server publish-microfrontend --source ./SomeMf --url https://feed.piral.cloud/api/v1/nuget/myfeed/index.json --key abcdef1234
```

This would publish the micro frontend contained in the `./SomeMf` directory to the `myfeed` feed of the publicly available community edition of the Piral Cloud Feed Service.

## Publishing using the `dotnet` CLI

Alternatively, either use the `nuget` or `dotnet` tool to publish the NuGet package:

```sh
dotnet nuget push SomeMf.nupkg --api-key abcdef1234 --source https://feed.piral.cloud/api/v1/nuget/myfeed/index.json
```

This works almost exactly the same as the `piral-blazor-server` tool, however, it expects you to have the build and pack command applied separately / beforehand.

## Publishing using Visual Studio

Finally, you can also publish a micro frontend using the [Publish NuGet package](https://learn.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli) feature of Visual Studio.

For this to work you need to have configured a special NuGet feed in Visual Studio using the URL and credentials that you've set up for your micro frontends feed in the Piral Cloud Feed Service.
