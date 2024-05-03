# HTTP Interceptors

Pilets can add *global* HTTP interceptors, which will be triggered for all HTTP requests using the *global* `HttpClient`. This can be done by adding a singleton `IHttpInterceptor` to the services, e.g.:

```cs
public class Module
{
    public static void Main() {}

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpInterceptor, MyInterceptor>();
    }
}
```

The interceptor itself has methods to react to a request (before sending the actual request) or response (after receiving the response).

```cs
class MyInterceptor : IHttpInterceptor
{
    public Task<HttpRequestMessage> OnRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Add some header
        request.Headers.Add("x-foo-bar", "other");
        return Task.FromResult(request);
    }

    public Task<HttpResponseMessage> OnResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        // Don't do anything
        return Task.FromResult(response);
    }
}
```

In case you want to intercept calls for injecting a bearer token obtained from calling the `getAccessCode()` pilet API you can just add a simple convenience service:

```cs
public class Module
{
    public static void Main() {}

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAccessCodeInterceptor();
    }
}
```

This way the current access code is retrieved and inserted into the request via the `Authorization` header.
