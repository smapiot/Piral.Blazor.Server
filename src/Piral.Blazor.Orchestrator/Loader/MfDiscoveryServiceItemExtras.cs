namespace Piral.Blazor.Orchestrator.Loader;

internal class MfDiscoveryServiceItemExtras
{
    public List<string>? Assemblies { get; set; }

    public Dictionary<string, string>? Dependencies { get; set; }

    public string? Id { get; set; }

    public string? Title { get; set; }

    public string? Summary { get; set; }

    public string? PojectUrl { get; set; }

    public string? LicenseUrl { get; set; }

    public string? Framework { get; set; }

    public MfDiscoveryServiceItemExtrasPilet? Pilet { get; set; }
}