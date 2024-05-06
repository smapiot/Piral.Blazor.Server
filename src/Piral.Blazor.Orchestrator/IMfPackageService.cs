namespace Piral.Blazor.Orchestrator;

public interface IMfPackageService
{
    Task<MicrofrontendPackage> LoadMicrofrontend(NugetEntryWithConfig entry);
}

