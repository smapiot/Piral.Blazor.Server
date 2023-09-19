namespace Piral.Blazor.Orchestrator
{
    public interface IEvents
    {
        void DispatchEvent<T>(string type, T args);

        void AddEventListener<T>(string type, Action<T> handler);

        void RemoveEventListener<T>(string type, Action<T> handler);
    }
}
