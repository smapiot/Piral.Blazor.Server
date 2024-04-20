# Components, Tiles, Menu Items, and Others

To register a Blazor component for use in the pilet API, the `PiralComponent` attribute can be used in two ways:

1. `[PiralComponent]`, this will register the component using the fully qualified name.
2. `[PiralComponent(<name>)]` will register the component using the custom name provided.

To register these components onto the pilet API, a `setup.tsx` file should be created at the root of your Blazor project.

This file may then, for example to register a tile, look like this:

```tsx
import { PiletApi } from '../piral~/<project_name>/node_modules/<piral_instance>';

type AddScript = (path: string, attrs?: Record<string, string>) => void;
type AddStyles = (path: string, pos?: 'first' | 'last' | 'before' | ' after') => void;

export default (app: PiletApi, addScript: AddScript, addStyles: AddStyles) => {
	//for a component marked with[PiralComponent("my-tile")]
	app.registerTile(app.fromBlazor('my-tile'));
};
```

The `addScript` function can be used to actually add more scripts, e.g.:

```tsx
export default (app: PiletApi, addScript: AddScript, addStyles: AddStyles) => {
	addScript("_content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js");
};
```

The first argument is the (relative) path to the RCL script, while the optional second argument provides additional attributes for the script to be added to the DOM.

The `addStyles` function can be used to add more style sheets, e.g.:

```tsx
export default (app: PiletApi, addScript: AddScript, addStyles: AddStyles) => {
  addStyles("_content/MudBlazor/MudBlazor.min.css");
};
```

**Important**: Non-abstract / exposed components with `PiralComponent` cannot have a type parameter. As these are directly instantiated from JavaScript there is no way to define the type to be used. As such, you cannot mark components as `@[PiralComponent]` and `@typeparam`. If you want to use a generic component, then wrap it (i.e., use a second component declared as a `PiralComponent`, which only mounts / renders the first component with the desired generic type).
