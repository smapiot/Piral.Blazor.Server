# Provider Components

Sometimes Blazor components require some global components (or "providers") to be added. To accomplish this you can create components marked with the `PiralProviderAttribute` attribute.

Example:


```razor
@attribute [PiralProvider]

<MudThemeProvider/>
<MudDialogProvider/>
<MudSnackbarProvider/>
```

Provider components are adjacent to your other components, which may come and go and will be - in general - somewhere else in the DOM. As such they are not ideal for providing some cascading value or other properties. They are ideal, however, when you need something running all the time.

In contrast, Piral also has the concept of a root component, which comes with another set of constraints.

::: tip
When should you use providers vs root components? In case you want something rendered (e.g., for the MudBlazor library) you should place the respective components in a provider. A root component makes sense if you want to use cascade value providers, i.e., something that is sensitive and only works if the cascade is correctly applied.
:::

Providers will *never* receive any parameters - they are rendered only once and will remain active for the whole lifecycle of the application. There can be more than one provider.
