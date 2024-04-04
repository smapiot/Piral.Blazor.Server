# Concepts

Generally, the terminology and ideas from [Piral](https://www.piral.io) have been transported over.

## App Shell / Piral Instance

An app shell (or Piral instance) is the main orchestration point. It must include the orchestrator, which is capable of loading the micro frontends and render their components.

For `Piral.Blazor.Core` (SPA) the app shell is a JavaScript project based on `piral` (npm package). It may already come with `piral-blazor` installed to provide Blazor functionality. Alternatively, micro frontends are regarded as standalone, with `piral-blazor` being used directly in the micro frontend.

For `Piral.Blazor.Orchestrator` (server) the app shell is your ASP.NET Core application referencing the NuGet package. It needs to be set up in the way that is described in this documentation.

## Micro Frontend / Pilet

A micro frontend (or pilet) is a module that can be loaded into an app shell. You can think of it as a plugin. A micro frontend can register / provide domain-specific components that are then rendered where specified. While a SPA micro frontend may come with all kinds of components (e.g., Angular, React, ... - not only Blazor), a server micro frontend can only register Blazor components.

## Micro Frontend Discovery / Feed Service

A micro frontend discovery service (or feed service) is a web service providing an API to publish or consume micro frontends. Usually, you want to access a certain feed that belongs to your application. A feed is a collection of micro frontends. Only users / systems that you allowed should be capable of publishing micro frontends (completely new or updated) to your feed.

A feed service is not required to operate `Piral.Blazor`, but we strongly recommend that you use some approach that allows such scaling. There is a [free community service](https://feed.piral.cloud) available from us. Alternatively, you can also [get a licensed Docker image](https://www.piral.cloud). For the latter you can also start with a free trial.

## Pilet API

Each micro frontend has a lifecycle that starts with a setup and ends with a teardown method. In the SPA method the whole lifecycle is provided in JavaScript, which is automatically generated and - ideally - does not need to be modified. For the server integration the lifecycle is contained in a class that inherits from `IMfModule`.

Once the setup method of your micro frontend is called you receive an object with some methods. This object (or more specifically the methods available on the object) are called the pilet API. It's the interface that can be used by a micro frontend to register its functionality in the app shell.

## Emulator

To allow local development of a micro frontend without needing to have or start the app shell locally there is a special mechanism known as an emulator. An emulator packages a dedicated debug build of the app shell in a package. For the SPA method this is an npm package, while for the server this is a NuGet package.

In any case the general idea is that you'll only need to reference this to debug your micro frontend (within the app shell - as you will later see it).

This idea is called emulator as it emulates a full runtime environment. You can also think of it as the runtime emulator from serverless environment (e.g., AWS Lambda, Azure Functions) or an emulator of your mobile phone OS (e.g., Android) when developing an app for it.
