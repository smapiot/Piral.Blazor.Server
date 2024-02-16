using System.Text.Json.Serialization;

namespace Piral.Blazor.Orchestrator.Loader;

internal class MfNugetServiceResource
{
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    [JsonPropertyName("@type")]
    public string? Type { get; set; }
}