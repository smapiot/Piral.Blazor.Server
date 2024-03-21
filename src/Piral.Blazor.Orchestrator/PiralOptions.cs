namespace Piral.Blazor.Orchestrator;

/// <summary>
/// Defines a set of options to configure the orchestrator.
/// </summary>
public class PiralOptions
{
    /// <summary>
    /// Assigns a list of isolated assemblies that will be
    /// part of the app shell, but should be loaded explicitly
    /// (if available) for the individual micro frontends.
    /// e.g., "My.Components.dll"
    /// </summary>
    public IEnumerable<string>? IsolatedAssemblies { get; set; }
}
