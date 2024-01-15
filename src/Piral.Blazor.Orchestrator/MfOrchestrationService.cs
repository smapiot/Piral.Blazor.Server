using Microsoft.Extensions.Hosting;

namespace Piral.Blazor.Orchestrator;

public class MfOrchestrationService : IHostedService
{
    private readonly IMfLoaderService _mfLoaderService;
    private readonly CancellationTokenSource _cts;

    public MfOrchestrationService(IMfLoaderService mfLoaderService)
    {
        _mfLoaderService = mfLoaderService;
        _cts = new CancellationTokenSource();
    }

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
