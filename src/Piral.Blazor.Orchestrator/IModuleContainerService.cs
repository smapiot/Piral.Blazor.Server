using Piral.Blazor.Shared;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public interface IModuleContainerService
{
    /// <summary>
    /// Configures the whole assembly.
    /// </summary>
    (IMfModule, IServiceProvider) ConfigureModule(Assembly assembly, JsonObject? config);

    /// <summary>
    /// Gets the provider that was established for the given assembly.
    /// Returns null if no provider has been established.
    /// </summary>
    IScopeResolver? GetProvider(Assembly assembly);
}
