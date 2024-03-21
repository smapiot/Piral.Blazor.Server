namespace Piral.Blazor.Orchestrator;

internal sealed class ProvidedPiralConfig(PiralOptions options) : IPiralConfig
{
    private readonly string[] _assemblies = (options.IsolatedAssemblies ?? Enumerable.Empty<string>()).ToArray();
    private readonly bool _emulator = Environment.GetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY") is not null;

    public bool IsEmulator => _emulator;

    public string[] IsolatedAssemblies => _assemblies;
}
