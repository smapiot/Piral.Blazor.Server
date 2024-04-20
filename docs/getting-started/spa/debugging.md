# Running and Debugging the Pilet

A micro frontend can be debugged using an IDE or the command line.

[[toc]]

## Using Visual Studio

The by far easiest way is to install the `Piral.Blazor.DevServer` package (as a replacement for the `Microsoft.AspNet.Components.WebAssembly.DevServer` package) and use F5 in Microsoft Visual Studio, JetBrains Rider, ... (essentially your IDE, if capable of running Blazor).

## Using the Command Line

Alternatively, from your Blazor project folder, you can run your pilet via the Piral CLI:

```sh
cd ../piral~/<project-name>
npm start
```

In addition to this, if you want to debug your Blazor pilet using for example Visual Studio, these requirements should be considered:

- keep the Piral CLI running
- debug your Blazor pilet using IISExpress

> :warning: if you want to run your pilet and directly visit it in the browser without debugging via IISExpress, you will have to disable a [kras](https://github.com/FlorianRappl/kras) script injector **before** visiting the pilet. To do this, go to `http://localhost:1234/manage-mock-server/#/injectors`, disable the `debug.js` script, and save your changes. Afterwards, you can visit `http://localhost:1234`.
