# Parameters

Parameters (or "props") are properly forwarded. Usually, it should be sufficient to declare `[Parameter]` properties in the Blazor components. Besides, there are more advanced ways.

For instance, to access the `params` prop of an extension you can use the `PiralParameter` attribute. This way, you can "forward" props from JS to the .NET name of your choice (in this case "params" is renamed to "Parameters").

```razor
@attribute [PiralExtension("sample-extension")]

<div>@Parameters.Test</div>

@code {
    public class MyParams
    {
        public string Test { get; set; }
    }

    [Parameter]
    [PiralParameter("params")]
    public MyParams Parameters { get; set; }
}
```

For the serialization you'll need to use either a `JsonElement` or something that can be serialized into. In this case, we used a class called `MyParams`.

**Important**: Make sure that your classes here are *serializable*, i.e., that they have a default / empty constructor (no parameters) and are public. Best case: These should be [POCOs](https://en.wikipedia.org/wiki/Plain_old_CLR_object).

With the `PiralParameter` you can also access / forward children to improve object access:

```razor
@attribute [PiralExtension("sample-extension")]

<div>@Message</div>

@code {
    [Parameter]
    [PiralParameter("params.Test")]
    public string Message { get; set; }
}
```

That way, we only have a property `Message` which reflects the `params.Test`. So if the extension is called like that:

```jsx
<app.Extension
    name="sample-extension"
    params={
        {
            Test: "Hello world",
        }
    }
/>
```

It would just work.
