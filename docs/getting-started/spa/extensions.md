# Extensions

To register an extension, the `PiralExtension` attribute can be used. You will also have to provide the extension slot name that defines where the extension should be rendered. The component can even be registered into multiple slots using multiple attributes.

```razor
//counter.razor

@attribute [PiralExtension("my-counter-slot")]
@attribute [PiralExtension("another-extension-slot")]

<h1>Counter</h1>

<p>Current count: @currentCount</p>

<button @onclick="IncrementCount">Click me</button>

@code {
    int currentCount = 0;

    void IncrementCount()
    {
        currentCount++;
    }
}
```

To use an extension within a Blazor component, the `<Extension>` component can be used.

```razor
<Extension Name="my-counter-slot"></Extension>
```

To pass in parameters to an extension component you can use the `Params` parameter. It expects an object with the actual parameters.

```razor
<Extension Name="react-counter" Params="new { count = 10, diff = 3 }" />
```

The other possibility is to actually render the provided extensions. This can be done by using the `Order` parameter together with a callback function.

```razor
<Extension Name="try-order" Order=@OrderExtensions />
```

The callback function receives a list of extension registrations, which would be rendered. They can be ordered or removed in any way - thus also acting as a filter. The resulting list of extensions needs to be returned. Example:

```cs
public List<ExtensionRegistration> OrderExtensions(List<ExtensionRegistration> components)
{
    return components.OrderBy(m => ((System.Text.Json.JsonElement)m.Defaults).GetProperty("order").GetInt32()).ToList();
}
```
