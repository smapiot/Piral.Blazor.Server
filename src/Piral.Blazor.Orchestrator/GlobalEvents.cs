using System.Collections.Concurrent;
using System.Text.Json;

namespace Piral.Blazor.Orchestrator;

internal class GlobalEvents : IEvents
{
    private ConcurrentDictionary<string, List<Delegate>> _eventMap = new();

    public void DispatchEvent<T>(string type, T args)
    {
        var handlers = _eventMap.GetValueOrDefault(type);

        if (handlers is not null)
        {
            // The dispatch is dynamic, try to convert
            if (args is JsonElement json)
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        var parameter = handler.Method.GetParameters().FirstOrDefault() ??
                            throw new InvalidOperationException("The delegate needs to have exactly one parameter.");
                        var argsType = parameter.ParameterType!;
                        var sargs = json.Deserialize(argsType) ??
                            throw new InvalidOperationException($"The provided args '{json}' could not be converted to a '{argsType.Name}'.");
                        handler.DynamicInvoke(sargs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while dispatching event '{0}' using args '{1}' to handler: {2}", type, json, ex);
                    }
                }
            }
            else
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.DynamicInvoke(args);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while dispatching event '{0}' to handler: {1}", type, ex);
                    }
                }
            }
        }
    }

    public void AddEventListener<T>(string type, Action<T> handler)
    {
        _eventMap.AddOrUpdate(type, (_) => [handler], (_, list) =>
        {
            list.Add(handler);
            return list;
        });
    }

    public void RemoveEventListener<T>(string type, Action<T> handler)
    {
        _eventMap.AddOrUpdate(type, (_) => [], (_, list) =>
        {
            list.Remove(handler);
            return list;
        });
    }
}
