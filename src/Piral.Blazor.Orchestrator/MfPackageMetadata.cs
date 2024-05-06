using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public class MfPackageMetadata : PackageMetadata
{
    public JsonObject? Config { get; set; } = null;
}
