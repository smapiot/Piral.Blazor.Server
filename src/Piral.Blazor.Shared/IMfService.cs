namespace Piral.Blazor.Shared;

/// <summary>
/// Represents a set of helper functions to be used in micro frontend components.
/// </summary>
public interface IMfService
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

	/// <summary>
	/// Gets the details for the current micro frontend.
	/// </summary>
	MfDetails Meta { get; }
}
