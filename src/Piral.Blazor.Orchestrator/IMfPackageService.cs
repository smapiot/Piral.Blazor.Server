namespace Piral.Blazor.Orchestrator;

public interface IMfPackageService
{
    Task<MicrofrontendPackage> LoadMicrofrontend(string name, string version);
}

