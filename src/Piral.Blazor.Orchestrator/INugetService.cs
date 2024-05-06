using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Piral.Blazor.Orchestrator;

public interface INugetService
{
    Task<PackageArchiveReader?> DownloadPackage(string packageName, string packageVersion);

    IEnumerable<PackageDependency> ListDependencies(PackageArchiveReader reader);

    Task<IEnumerable<PackageMetadata>> RetrieveDependencies(string packageName, string packageVersion);
}
