namespace Piral.Blazor.Orchestrator;

/// <summary>
/// Represents the micro frontend shared event basis.
/// </summary>
public interface IEvents
{
    /// <summary>
    /// Dispatches an event using the given type.
    /// </summary>
    void DispatchEvent<T>(string type, T args);

    /// <summary>
    /// Adds the provided event listener.
    /// </summary>
    void AddEventListener<T>(string type, Action<T> handler);

    /// <summary>
    /// Removes the provided event listener.
    /// </summary>
    void RemoveEventListener<T>(string type, Action<T> handler);
}
