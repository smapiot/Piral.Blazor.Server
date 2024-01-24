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

	/// <summary>
	/// Tries to set some data available across the micro frontends.
	/// </summary>
	/// <typeparam name="T">The type of the data to set.</typeparam>
	/// <param name="name">The name of the data entry.</param>
	/// <param name="value">The data value to store.</param>
	/// <returns>True if the data was set, otherwise false.</returns>
	bool TrySetData<T>(string name, T value);

    /// <summary>
    /// Tries to get some data available for all micro frontends.
    /// </summary>
    /// <typeparam name="T">The type of the data to retrieve.</typeparam>
    /// <param name="name">The name of the data entry.</param>
    /// <param name="value">The available data value, if any.</param>
    /// <returns>True if the data was available, otherwise false.</returns>
    bool TryGetData<T>(string name, out T value);
}
