# Standard Micro Frontend Service

Every pilet gets automatically a service called `IMfService` injected.

[[toc]]

## Global Events

You can use the `IMfService` service to emit and receive events via the standard API event bus. This is great for doing loosely-coupled MF-to-MF communication.

Example:

```razor
@inject IMfService ps
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

Another component can now trigger this by using `ps.DispatchEvent("toggle-sidebar", false);` with an injected `@inject IMfService ps`.

**Important**: This service is not scoped! So the communication is done directly, without taking the current user context (if any) into consideration. If you require the user context to play a role, you should consider using scoped events, which can be done via the `IScopedEvents` service.

## Scoped Events

For using scoped events you will need to inject the `IScopedEvents` into the context:

```razor
@inject IScopedEvents events

<div @onclick=@Example>...</div>

@code {
    public void Example()
    {
        events.DispatchEvent<string>("clicked-button", "Content");
    }
}
```

Remember that the `IMfService` is not scoped - so for the scoped events you will need to use a different service.

## Sharing Data

The `IMfService` can be used to access some shared memory storage. The functions `TrySetData` and `TryGetData` can be used to set and get data on the shared storage.

Here's an example:

```razor
@inject IMfService ps

<button @onclick=@StoreValue>Store some value</button>

<button @onclick=@LogValue>Log current value</button>

@code {
    public async void StoreValue()
    {
        if (ps.TrySetData<string>("myValue", "Hello World!"))
        {
            Console.WriteLine("Stored new value: {0}", value);
        }
        else
        {
            Console.WriteLine("Could not store value.");
        }
    }

    public async void LogValue()
    {
        if (ps.TryGetData<string>("myValue", out var value))
        {
            Console.WriteLine("Currently stored value is: {0}", value);
        }
        else
        {
            Console.WriteLine("Currently nothing stored.");
        }
    }
}
```

Note that only the micro frontend that initially wrote the data is capable of updating it. Conflicts are therefore avoided by giving only a single micro frontend right access - following the first-write-claims-ownership principle.
