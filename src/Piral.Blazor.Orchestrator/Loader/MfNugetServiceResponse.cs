using System.Text.Json.Serialization;

namespace Piral.Blazor.Orchestrator.Loader;

internal class MfNugetServiceResponse
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("resources")]
    public List<MfNugetServiceResource>? Resources { get; set; }
}
