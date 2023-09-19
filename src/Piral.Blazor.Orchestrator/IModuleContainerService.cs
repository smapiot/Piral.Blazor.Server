using Piral.Blazor.Shared;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public interface IModuleContainerService
{
    /// <summary>
    /// Configures the whole assembly.
    /// </summary>
    Task<IMfModule> ConfigureModule(Assembly assembly, IMfAppService app);

    /// <summary>
    /// Gets the provider that was established for the given assembly.
    /// Returns null if no provider has been established.
    /// </summary>
    IScopeResolver? GetProvider(Assembly assembly);
}
