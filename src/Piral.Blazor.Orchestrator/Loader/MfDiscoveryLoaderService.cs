using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Piral.Blazor.Orchestrator.Loader;

public class MfDiscoveryLoaderService : IMfLoaderService
{
    private readonly HttpClient _client;
    private readonly IMfRepository _repository;
    private readonly IMfPackageService _package;
    private readonly ISnapshotService _snapshot;
    private readonly string _feedUrl;
    private readonly string _wsUrl;

    public MfDiscoveryLoaderService(IHttpClientFactory client, IMfRepository repository, IMfPackageService package, ISnapshotService snapshot, IConfiguration configuration)
    {
        _client = client.CreateClient();
        _repository = repository;
        _snapshot = snapshot;
        _package = package;
        _feedUrl = configuration.GetValue<string>("Microfrontends:DiscoveryInfoUrl")!;
        _wsUrl = configuration.GetValue<string>("Microfrontends:DiscoveryUpdateUrl")!;

        repository.PackagesChanged += OnPackagesChanged;
    }

    private void OnPackagesChanged(object? sender, EventArgs e)
    {
        var ids = _repository.Packages
            .Select(m => new NugetEntry { Name = m.Name, Version = m.Version })
            .Select(m => m.MakePackageId());

        _snapshot.UpdateMicrofrontends(ids);
    }

    public async Task LoadMicrofrontends(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_feedUrl))
        {
            var response = await _client.GetFromJsonAsync<MfDiscoveryServiceResponse>(_feedUrl);

            if (response?.MicroFrontends is not null)
            {
                foreach (var item in response.MicroFrontends)
                {
                    var data = item.Value.FirstOrDefault();
                    var version = data?.Metadata?.Version;

                    if (version is not null)
                    {
                        var mf = await _package.LoadMicrofrontend(item.Key, version);
                        await _repository.SetPackage(mf);
                    }
                }
            }
        }
    }

    public async void ConnectMicrofrontends(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && !string.IsNullOrEmpty(_wsUrl))
        {
            using var ws = new ClientWebSocket();

            try
            {
                await ws.ConnectAsync(new Uri(_wsUrl), ct).ConfigureAwait(false);
                await Subscribe(ws, ct).ConfigureAwait(false);
            }
            catch
            {
                // Ignore such errors for now - just reconnect as long as
                // the service is running
                // We wait a second to give the server a chance to recover
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }

    private async Task Subscribe(ClientWebSocket ws, CancellationToken ct)
    {
        while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var res = await ListenForResponse(ws, ct).ConfigureAwait(false);
            var item = JsonSerializer.Deserialize<MfDiscoveryServiceEvent>(res, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
            });

            if (item?.Type == "update-pilet" || item?.Type == "add-pilet" || item?.Type == "delete-pilet")
            {
                await LoadMicrofrontends(ct).ConfigureAwait(false);
            }
        }
    }

    private static async Task<string> ListenForResponse(ClientWebSocket ws, CancellationToken ct)
    {
        var receiveBuffer = new byte[256];
        var offset = 0;
        var dataPerPacket = 10;
        var result = new WebSocketReceiveResult(0, WebSocketMessageType.Text, false);

        while (!result.EndOfMessage)
        {
            var bytesReceived = new ArraySegment<byte>(receiveBuffer, offset, dataPerPacket);
            result = await ws.ReceiveAsync(bytesReceived, ct).ConfigureAwait(false);
            offset += result.Count;
        }

        return Encoding.UTF8.GetString(receiveBuffer, 0, offset);
    }
}
