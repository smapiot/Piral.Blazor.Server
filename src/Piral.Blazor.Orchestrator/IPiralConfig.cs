namespace Piral.Blazor.Orchestrator;

public interface IPiralConfig
{
    bool IsEmulator { get; }

    string[] IsolatedAssemblies { get; }
}
