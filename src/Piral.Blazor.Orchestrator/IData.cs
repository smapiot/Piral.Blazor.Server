namespace Piral.Blazor.Orchestrator;

/// <summary>
/// Represents the micro frontend shared data basis.
/// </summary>
public interface IData
{
    /// <summary>
    /// Tries to set some data available across the micro frontends.
    /// </summary>
    /// <typeparam name="T">The type of the data to set.</typeparam>
    /// <param name="owner">The name of the micro frontend storing the data.</param>
    /// <param name="name">The name of the data entry.</param>
    /// <param name="value">The data value to store.</param>
    /// <returns>True if the data was set, otherwise false.</returns>
    bool TrySetData<T>(string owner, string name, T value);

    /// <summary>
    /// Tries to get some data available for all micro frontends.
    /// </summary>
    /// <typeparam name="T">The type of the data to retrieve.</typeparam>
    /// <param name="requester">The name of the micro frontend requesting the data.</param>
    /// <param name="name">The name of the data entry.</param>
    /// <param name="value">The available data value, if any.</param>
    /// <returns>True if the data was available, otherwise false.</returns>
    bool TryGetData<T>(string requester, string name, out T value);
}
