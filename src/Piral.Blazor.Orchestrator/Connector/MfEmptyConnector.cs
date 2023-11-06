
using Microsoft.AspNetCore.Http;

namespace Piral.Blazor.Orchestrator.Connector;

internal class MfEmptyConnector : IMfDebugConnector
{
    private readonly IEnumerable<string> empty = Enumerable.Empty<string>();
    private readonly Task<bool> notIntercepted = Task.FromResult(false);

    public IEnumerable<string> Styles => empty;

    public IEnumerable<string> Scripts => empty;

    public Task<bool> InterceptAsync(HttpContext _) => notIntercepted;
}
