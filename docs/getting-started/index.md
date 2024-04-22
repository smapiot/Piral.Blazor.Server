# Getting Started

`Piral.Blazor` exists in multiple forms. Right now, we have:

- `Piral.Blazor.Core`, which is made to be consumed in a JavaScript / SPA created with the [Piral](https://docs.piral.io) framework
- `Piral.Blazor.Orchestrator`, which is made to be consumed in an ASP.NET Core web application to enable server-side micro frontends

While both are fundamentally different, they share the same goals and have an overlapping development philosophy.

::: info
`Piral.Blazor.Core` is never directly referenced, you'd rather install the [`blazor`](https://www.npmjs.com/package/blazor) npm package.
:::

In the future there will be an overlap between the two, for now you need to decide in the beginning. Usually, the decision is rather simple - it boils down to:

- If you have an existing Blazor server application that you now want to continue developing using a micro frontend approach then use `Piral.Blazor.Orchestrator`
- If you have an existing SPA that you want to extend with some component written in Blazor then aim for the Piral framework with `Piral.Blazor.Core`

Generally, `Piral.Blazor.Orchestrator` will allow you to do everything in Blazor while `Piral.Blazor.Core` will require you to work with TypeScript and other technologies such as React, too.
