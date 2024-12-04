namespace Piral.Blazor.Shared;

/// <summary>
/// Represents an event bus that works globally - i.e., beyond the session scope.
/// </summary>
public interface IGlobalEvents
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
