using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator.Loader;

public class MfDiscoveryLoaderService : IMfLoaderService
{
    private readonly HttpClient _client;
    private readonly IMfRepository _repository;
    private readonly IMfPackageService _package;
    private readonly ISnapshotService _snapshot;
    private readonly string _feedUrl;
    private readonly string _wsUrl;
    private readonly List<MfPackageMetadata> _current = [];

    public MfDiscoveryLoaderService(IHttpClientFactory client, IMfRepository repository, IMfPackageService package, ISnapshotService snapshot, IConfiguration configuration)
    {
        var configuredFeedUrl = configuration.GetValue<string>("Microfrontends:DiscoveryInfoUrl");
        var configuredWsUrl = configuration.GetValue<string>("Microfrontends:DiscoveryUpdateUrl");
        _client = client.CreateClient();
        _repository = repository;
        _snapshot = snapshot;
        _package = package;
        _feedUrl = configuredFeedUrl ?? "https://feed.piral.cloud/api/v1/microfrontends/empty";
        _wsUrl = configuredWsUrl ?? "wss://feed.piral.cloud/api/v1/pilet/empty";

        repository.PackagesChanged += OnPackagesChanged;
    }

    private void OnPackagesChanged(object? sender, EventArgs e)
    {
        var entries = _repository.Packages
            .Select(m => new MfPackageMetadata { Name = m.Name, Version = m.Version, Config = m.Config });

        _snapshot.UpdateMicrofrontends(entries);
    }

    public async Task LoadMicrofrontends(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_feedUrl))
        {
            // This is most likely a NuGet feed
            if (_feedUrl.EndsWith("/index.json"))
            {
                var response = await _client.GetFromJsonAsync<MfNugetServiceResponse>(_feedUrl, cancellationToken);
                var discoveryUrl = response?.Resources?.FirstOrDefault(m => m.Type == "MicroFrontendDiscovery/1.0.0")?.Id;

                if (discoveryUrl is not null)
                {
                    await LoadMicrofrontendsFromDiscoveryUrl(discoveryUrl, cancellationToken);
                }
            }
            else
            {
                await LoadMicrofrontendsFromDiscoveryUrl(_feedUrl, cancellationToken);
            }
        }
    }

    private async Task LoadMicrofrontendsFromDiscoveryUrl(string feedUrl, CancellationToken cancellationToken)
    {
        var response = await _client.GetFromJsonAsync<MfDiscoveryServiceResponse>(feedUrl, cancellationToken);
        var next = new List<MfPackageMetadata>();

        if (response?.MicroFrontends is not null)
        {
            foreach (var item in response.MicroFrontends)
            {
                var data = item.Value.FirstOrDefault();
                var version = data?.Metadata?.Version;

                if (version is not null)
                {
                    var name = data?.Extras?.Id ?? item.Key;
                    var config = data?.Extras?.Config;
                    next.Add(new MfPackageMetadata
                    {
                        Config = config,
                        Name = name,
                        Version = version,
                    });
                }
            }
        }

        var (updated, removed) = GetDiff(next);

        foreach (var entry in removed)
        {
            await _repository.DeletePackage(entry.Name);
        }

        foreach (var entry in updated)
        {
            var mf = await _package.LoadMicrofrontend(entry);
            await _repository.SetPackage(mf);
        }
    }

    private (IEnumerable<MfPackageMetadata>, IEnumerable<MfPackageMetadata>) GetDiff(List<MfPackageMetadata> next)
    {
        if (_current.Count == 0)
        {
            return (next, Enumerable.Empty<MfPackageMetadata>());
        }

        var removed = new List<MfPackageMetadata>();
        var updated = new List<MfPackageMetadata>(next);

        for (var i = _current.Count - 1; i >= 0; i--)
        {
            var currentEntry = _current[i];
            var matchingEntry = next.FirstOrDefault(m => m.Name == currentEntry.Name);

            if (matchingEntry is null)
            {
                removed.Add(currentEntry);
                _current.RemoveAt(i);
            }
            else if (matchingEntry.Version != currentEntry.Version)
            {
                _current[i] = matchingEntry;
            }
            else if (IsDifferent(matchingEntry.Config, currentEntry.Config))
            {
                _current[i] = matchingEntry;
            }
            else
            {
                updated.Remove(matchingEntry);
            }
        }

        return (updated, removed);
    }

    private static bool IsDifferent(JsonObject? config1, JsonObject? config2) => JsonObject.DeepEquals(config1, config2);

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
                await Task.Delay(1000, ct).ConfigureAwait(false);
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
