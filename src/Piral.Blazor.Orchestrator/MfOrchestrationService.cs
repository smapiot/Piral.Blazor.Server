using Microsoft.Extensions.Hosting;

namespace Piral.Blazor.Orchestrator;

public class MfOrchestrationService(IMfLoaderService mfLoaderService) : IHostedService
{
    private readonly IMfLoaderService _mfLoaderService = mfLoaderService;
    private readonly CancellationTokenSource _cts = new();

    public async Task StartAsync(CancellationToken ct)
    {
        await _mfLoaderService.LoadMicrofrontends(ct);
        _mfLoaderService.ConnectMicrofrontends(_cts.Token);
    }

    public Task StopAsync(CancellationToken ct)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
