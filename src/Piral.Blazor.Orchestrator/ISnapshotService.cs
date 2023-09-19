using NuGet.Packaging;

namespace Piral.Blazor.Orchestrator;

public interface ISnapshotService
{
    Task<IEnumerable<string>> AvailableMicrofrontends();

    Task UpdateMicrofrontends(IEnumerable<string> ids);

    Task<PackageArchiveReader?> LoadPackage(string id);

    Task<IEnumerable<NugetEntry>> ListDependencies(string id);
}
