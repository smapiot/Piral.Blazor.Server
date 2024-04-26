using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public class NugetEntry
{
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}

public class NugetEntryWithConfig : NugetEntry
{
    public JsonObject? Config { get; set; } = null;
}
