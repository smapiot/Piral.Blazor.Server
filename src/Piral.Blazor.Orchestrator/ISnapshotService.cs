using NuGet.Packaging;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public interface ISnapshotService
{
    Task<IEnumerable<string>> AvailableMicrofrontends();

    Task UpdateMicrofrontends(IEnumerable<string> ids);

    Task<JsonObject?> GetConfig(string id);

    Task<PackageArchiveReader?> LoadPackage(string id);

    Task<IEnumerable<NugetEntry>> ListDependencies(string id);
}
