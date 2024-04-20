# Localization

Localization works almost exactly as with standard Blazor, except that the language can be changed at runtime directly rather then requiring a full reload of the page.

The other difference is that the initial language is no longer decided by the server's response headers, but rather by the app shell. The initial configuration options of the `piral-blazor` plugin allow setting the `initialLanguage`. These options also allow setting up a callback to decide when to change the language (and to what language). If not explicitly stated Blazor will just listen to the `select-language` event of Piral, providing a key `currentLanguage` in the event arguments.

To dynamically change / refresh your components when the language change you'll need to listen to the `LanguageChanged` event emitted by the injected `IPiletService` instance:

```razor
@inject IStringLocalizer<MyComponent> loc
@inject IPiletService pilet

<h2>@loc["greeting"]</h2>

@code {
    protected override void OnInitialized()
    {
        pilet.LanguageChanged += (s, e) => this.StateHasChanged();
        base.OnInitialized();
    }
}
```

This way, your components will always remain up-to-date and render the right translations.
