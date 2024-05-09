using NuGet.Frameworks;

namespace Piral.Blazor.Orchestrator;

internal static class Constants
{
    public const string Target = "net8.0";
    public static readonly NuGetFramework CurrentFramework = NuGetFramework.Parse(Target);
}
