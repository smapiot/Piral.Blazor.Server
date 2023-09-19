namespace Piral.Blazor.Orchestrator;

public interface IMfRepository
{
    event EventHandler? PackagesChanged;

    IEnumerable<MicrofrontendPackage> Packages { get; }

	Task SetPackage(MicrofrontendPackage package);

	Task DeletePackage(string name);
}
