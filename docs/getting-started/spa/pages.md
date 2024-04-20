# Pages

A standard page in Blazor, using the `@page` directive, will work as expected, and will be automatically registered on the pilet API.

You can also set multiple `@page` directives, which will all lead to page registrations.

[[toc]]

## Route Parameters

If you want to match the route parameter you can use the generic approach, too:

```razor
@page "/foo/{id}"

<div>@Id</div>

@code {
    [Parameter]
    [PiralParameter("match.params.id")]
    public string Id { get; set; }
}
```

However, since using `match.params` is quite verbose and easy to get wrong you can also use the special `PiralRouteParameter` attribute.

```razor
@page "/foo/{id}"

<div>@Id</div>

@code {
    [Parameter]
    [PiralRouteParameter("id")]
    public string Id { get; set; }
}
```

Note that there is another convenience deriving from the use of `PiralRouteParameter`. If your route parameter name matches the name of the property then you can also omit the argument:

```razor
@page "/foo/{Id}"

<div>@Id</div>

@code {
    [Parameter]
    [PiralRouteParameter]
    public string Id { get; set; }
}
```

## Query Parameters

In addition to route parameters you can also match the query parameters using the `PiralQueryParameter` attribute:

```cs
@page "/foo"

<div>@Id</div>

@code {
    [Parameter]
    [PiralQueryParameter]  
    public string Id { get; set; } 
}
```

The previous example would match `/foo?id=bar` with `Id` being set to `bar`. You could also change the name of the used query parameter:

```cs
@page "/foo"

<div>@SearchQuery</div>

@code {
    [Parameter]
    [PiralQueryParameter("q")]  
    public string SearchQuery { get; set; } 
}
```

This would print `hello` for `/foo?q=hello`.
