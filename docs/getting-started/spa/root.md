# Root Component

By default, the Blazor pilets run in a dedicated Blazor application with no root component. If you need a root component, e.g., to provide some common values from a `CascadingValue` component such as `CascadingAuthenticationState` from the `Microsoft.AspNetCore.Components.Authorization` package, you can actually override the default root component:

```razor
@attribute [PiralAppRoot]

<CascadingAuthenticationState>
    @ChildContent
</CascadingAuthenticationState>

@code {
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}
```

You can also provide your own providers here (or nest them as you want):

```razor
@attribute [PiralAppRoot]

<CascadingValue Value="@theme">
    <div>
        @ChildContent
    </div>
</CascadingValue>

@code {
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    
    private string theme = "dark";
}
```

It is critical to understand that each attached pilet component starts its own Blazor rendering tree. Therefore, while there is just a single `PiralAppRoot` component there might be multiple instances active at a given point in time. This is a crucial difference to `PiralProvider` components, which are essentially singletons from a rendering perspective.

::: warning
There is always just one `PiralAppRoot` component. If you did not supply one then the default `PiralAppRoot` will be used. If you already provided one, no other `PiralAppRoot` can be used.
:::

The difference between app root and provider components is as follows:

- The `PiralAppRoot` attribute can only be declared once and will be instantiated per exposed component (i.e., per Blazor render tree), i.e., single declaration, multi instance
- The `PiralProvider` attribute can be used as often as you want, but every component using it will be essentially be a singleton at runtime - living in the blazor-root, i.e., multi declaration, single instance
