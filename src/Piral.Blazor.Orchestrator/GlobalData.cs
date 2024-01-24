using System.Collections.Concurrent;
using System.Text.Json;

namespace Piral.Blazor.Orchestrator;

internal class GlobalData : IData
{
    private ConcurrentDictionary<string, DataEntry> _data = new();

    public bool TryGetData<T>(string owner, string name, out T value)
    {
        value = default!;

        if (_data.TryGetValue(name, out var item))
        {
            if (item.Value is T val)
            {
                value = val;
            }
            else
            {
                var original = JsonSerializer.Serialize(item.Value);
                value = JsonSerializer.Deserialize<T>(original)!;
            }

            return true;
        }

        return false;
    }

    public bool TrySetData<T>(string owner, string name, T value)
    {
        if (!_data.TryGetValue(name, out var item))
        {
            return _data.TryAdd(name, new DataEntry(owner, value));
        }
        else if (item.Owner == owner)
        {
            return _data.TryUpdate(name, new DataEntry(owner, value), item);
        }
        else
        {
            return false;
        }
    }

    record struct DataEntry(string Owner, object? Value);
}
