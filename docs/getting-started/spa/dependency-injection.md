# Dependency Injection

You can define services for dependency injection in a `Module` class. The name of the class is arbitrary, but it shows the difference to the standard `Program` class, which should not be available, as mentioned before.

To be able to compile successfully, a `Main` method should be declared, which should remain empty.

```cs
public class Module
{
    public static void Main()
    {
        // this entrypoint should remain empty
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        // configure dependency injection for the components in the pilet here
    }
}
```

The `ConfigureServices` method is optional. If you want to configure dependency injection in your pilet then use this.

If a third-party library requires globally shared dependencies (or global injected DI) then add it to a global pilet (setting the `PiletKind` to `global` in the csproj / build configuration).

Additionally, the `ConfigureServices` method supports another argument providing the configuration of the pilet, i.e., the `IConfiguration` object. So, the example above could be rewritten to be:

```cs
public class Module
{
    public static void Main()
    {
        // this entrypoint should remain empty
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }
}
```

The configuration uses the `meta.config` of the Pilet API provided by the pilet.

**Important**: There is no support for the *appsettings...json* file as the configuration is assumed to be distributed. Use the `meta.config` approach described below for local development and a proper feed service with configuration support for production purposes.
