using System.Reflection;

namespace Piral.Blazor.Orchestrator;

internal interface ICacheManipulatorService
{
    void UpdateComponentCache(Assembly assembly);
}