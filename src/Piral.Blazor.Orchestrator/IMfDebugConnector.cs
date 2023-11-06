using Microsoft.AspNetCore.Http;

namespace Piral.Blazor.Orchestrator;

public interface IMfDebugConnector
{
	IEnumerable<string> Styles { get; }

    IEnumerable<string> Scripts { get; }

    Task<bool> InterceptAsync(HttpContext context);
}
