using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public interface ICacheManipulatorService
{
    void UpdateComponentCache(Assembly assembly);
}