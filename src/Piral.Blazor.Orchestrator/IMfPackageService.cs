using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public interface IMfPackageService
{
    Task<MicrofrontendPackage> LoadMicrofrontend(string name, string version, JsonObject? config);
}

