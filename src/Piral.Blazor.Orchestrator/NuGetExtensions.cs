using NuGet.Frameworks;
using NuGet.Packaging;

namespace Piral.Blazor.Orchestrator;

internal static class NuGetExtensions
{
    private static bool CheckCompatibility(FrameworkSpecificGroup group) => !group.TargetFramework.IsAny && IsCompatible(group.TargetFramework);

    private static bool IsCompatible(NuGetFramework framework) => DefaultCompatibilityProvider.Instance.IsCompatible(Constants.CurrentFramework, framework);

    public static IEnumerable<string> GetMatchingLibItems(this PackageArchiveReader package)
    {
        var libItems = package.GetLibItems().FirstOrDefault(CheckCompatibility)?.Items;

        if (libItems is null)
        {
            return package.GetLibItems().FirstOrDefault(m => m.TargetFramework.IsAny)?.Items ?? Enumerable.Empty<string>();
        }

        return libItems;
    }
}
