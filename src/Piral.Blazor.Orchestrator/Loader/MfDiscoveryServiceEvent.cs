using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator.Loader;

internal class MfDiscoveryServiceEvent
{
    public string? Type { get; set; }

    public JsonObject? Data { get; set; }
}

