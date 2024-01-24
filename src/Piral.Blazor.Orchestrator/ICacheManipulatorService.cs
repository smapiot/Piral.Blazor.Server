namespace Piral.Blazor.Orchestrator;

public interface ICacheManipulatorService
{
    void UpdateComponentCache(Type componentType, IScopeResolver resolver);
}