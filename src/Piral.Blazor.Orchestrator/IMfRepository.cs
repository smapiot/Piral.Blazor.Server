namespace Piral.Blazor.Orchestrator;

public interface IMfRepository
{
    event EventHandler? PackagesChanged;

    IEnumerable<MicrofrontendPackage> Packages { get; }

    MicrofrontendPackage? GetPackage(string name);

    Task SetPackage(MicrofrontendPackage package);

    Task DeletePackage(string name);
}
