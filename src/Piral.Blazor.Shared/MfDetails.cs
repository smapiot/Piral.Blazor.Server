using System.Text.Json.Nodes;

namespace Piral.Blazor.Shared;

/// <summary>
/// Contains details about a micro frontend.
/// </summary>
public record MfDetails
{
    /// <summary>
    /// Gets the name of the micro frontend.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the version of the micro frontend.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets the configuration of the micro frontend.
    /// </summary>
    public required JsonObject Config { get; init; }
}
