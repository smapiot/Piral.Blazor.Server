# Environment Configuration

Runtime configuration works different for micro frontends. This is mostly true due to the fact that micro frontends are distributed, whereas the classic way is a configuration found at a central location such as a file or a some environment variables at a single server.

Nevertheless, at the end all configuration (like the rest of the code) has to end up in the application. Therefore, the configuration is assumed to be coming *with* the micro frontend. For this we rely on the response from the micro frontend discovery service (e.g., the Piral Cloud Feed Service) to also bring in configuration.

The Piral framework takes care of understanding the discovery service response and dispatching the delivered configuration to the micro frontend. In the micro frontend you can obtain the configuration via the `IConfiguration` passed into the `ConfigureServices` section:

```cs
public class Module
{
    public static void Main() {}

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Use IConfiguration
    }
}
```

As the discovery service might set the configuration for published micro frontends, the debug issue for local development still exists. How do you get in configuration in here?

To make local development work you can enhance the response of the local debug server (mocking a micro frontend discovery service). With the `meta.json` file you can add additional sections to the micro frontend's meta data:

```json
{
    "config": {
        "foo": "bar"
    }
}
```

Using this content you added a `config` section to the meta response of the local debug server returning you the provided configuration *locally*.
