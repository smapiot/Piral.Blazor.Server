# Standard Pilet Service

Every pilet gets automatically a service called `IPiletService` injected.

[[toc]]

## Asset URLs

The `IPiletService` service can be used to compute the URL of a resource.

```razor
@inject IPiletService Pilet
```

The relevant helper method is `GetUrl`. You can use it like:

```razor
@page "/example"
@inject IPiletService Pilet

<img src=@Pilet.GetUrl("images/something.png") alt="Some image" />
```

In the example above the resource `images/something.png` would be placed in the `wwwroot` folder (i.e., `wwwroot/images/something`). As the content of the `wwwroot` folder is copied, the image will also be copied. However, the old local URL is not valid in a pilet, which needs to prefix its resources with its base URL. The function above does that. In that case, the URL would maybe be something like `http://localhost:1234/$pilet-api/0/images/something.png` while debugging, and another fully qualified URL later in production.

## Events

You can use the `IPiletService` service to emit and receive events via the standard Pilet API event bus. This is great for doing loosely-coupled pilet-to-pilet communication.

Example:

```razor
@attribute [PiralComponent]
@inject IPiletService ps
@implements IDisposable

<aside class=@_sidebarClass>
  <a @onclick=@CloseSidebar style="display: inline-block; padding: 0 10px; cursor: pointer;">x</a>
</aside> 

@code {
    [Parameter]
    public bool IsOpen { get; set; } = false;

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    string _sidebarClass { get => IsOpen ? "sidebar open" : "sidebar"; }

    public void Dispose()
    {
        ps.RemoveEventListener<bool>("toggle-sidebar", ToggleSidebar);
    }

    protected override void OnInitialized()
    {
        ps.AddEventListener<bool>("toggle-sidebar", ToggleSidebar);
    }

    public void ToggleSidebar(bool value) => IsOpenChanged.InvokeAsync(value);

    public void CloseSidebar() => ToggleSidebar(false);
}
```

Another component can now trigger this by using `ps.DispatchEvent("toggle-sidebar", false);` with an injected `@inject IPiletService ps`.

## Pilet Data and API Access

You can use the `IPiletService` service to call methods living on the pilet API. This makes mostly sense for APIs that are quite primitive, e.g., accepting and returning only strings, booleans, and integers.

In general this is working via the `Call` API. An example would be:

```razor
@attribute [PiralComponent]
@inject IPiletService ps

<button @onclick=@LogValue>Log current value</button>

@code {
    public async void LogValue()
    {
      var value = await ps.Call<string>("getData", "myValue");
      Console.WriteLine("Currently stored value is: {0}", value);
    }
}
```

For some more common pilet API functions extension methods exist. The call beforehand to the `getData` function could be simplified with the `GetDataValue` extension:

```razor
@attribute [PiralComponent]
@inject IPiletService ps

<button @onclick=@LogValue>Log current value</button>

@code {
    public async void LogValue()
    {
      var value = await ps.GetDataValue<string>("myValue");
      Console.WriteLine("Currently stored value is: {0}", value);
    }
}
```
