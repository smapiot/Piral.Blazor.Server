using System.Collections.Concurrent;

namespace Piral.Blazor.Orchestrator;

internal class GlobalEvents : IEvents
{
    private ConcurrentDictionary<string, List<Delegate>> _eventMap = new();

    public void DispatchEvent<T>(string type, T args)
    {
        var handlers = _eventMap.GetValueOrDefault(type);

        if (handlers is not null)
        {
            foreach (var handler in handlers)
            {
                handler.DynamicInvoke(args);
            }
        }
    }

    public void AddEventListener<T>(string type, Action<T> handler)
    {
        _eventMap.AddOrUpdate(type, (_) => new List<Delegate> { handler }, (_, list) =>
        {
            list.Add(handler);
            return list;
        });
    }

    public void RemoveEventListener<T>(string type, Action<T> handler)
    {
        _eventMap.AddOrUpdate(type, (_) => new List<Delegate> {}, (_, list) =>
        {
            list.Remove(handler);
            return list;
        });
    }
}
