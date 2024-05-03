# Environment Configuration

Runtime configuration works different for micro frontends. This is mostly true due to the fact that micro frontends are distributed, whereas the classic way is a configuration found at a central location such as a file or a some environment variables at a single server.

Nevertheless, at the end all configuration (like the rest of the code) has to end up in the application. Therefore, the configuration is assumed to be coming *with* the micro frontend. For this we rely on the response from the micro frontend discovery service (e.g., the Piral Cloud Feed Service) to also bring in configuration.

The Piral framework takes care of understanding the discovery service response and dispatching the delivered configuration to the micro frontend. In the micro frontend you can obtain the configuration via the `IMfService` or `IMfAppService` instances, e.g., passed into the setup lifecycle:

```cs
public class Module : IMfModule
{
    public Task Setup(IMfAppService app)
    {
        // use: app.Meta.Config
    }
    
    public Task Teardown(IMfAppService app)
    {
    }
    
    public void Configure(IServiceCollection services)
    {
    }
}
```

As the discovery service might set the configuration for published micro frontends, the debug issue for local development still exists. How do you get in configuration in here?

To make local development work you can add a `config.json` file to your micro frontend, which has to be copied to the output directory. This file can now contain a valid configuration as JSON:

```json
{
    "foo": "bar"
}
```

This file is automatically picked up by the emulator when you start debugging your micro frontend.
