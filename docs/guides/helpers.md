# Helpers

All utilities are available in the `Piral.Blazor.Shared` package.

Right now this only contains extension methods and the interfaces used for [dependency injection](../getting-started/server/mf-service.md).

The `GetLink` extension method (for an `IMfService` instance) can be used to transform given relative local URL to a global micro frontend URL, e.g., for `my-image.jpg` it would return `/assets/[mf-name]/my-image.jpg`.
