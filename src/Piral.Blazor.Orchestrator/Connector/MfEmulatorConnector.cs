using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Piral.Blazor.Orchestrator.Connector;

internal class MfEmulatorConnector : IMfDebugConnector
{
    private readonly IEnumerable<string> _styles = Enumerable.Empty<string>();
    private readonly IEnumerable<string> _scripts = new[] { "_content/Piral.Blazor.Orchestrator/debug.js" };

    public IEnumerable<string> Styles => _styles;

    public IEnumerable<string> Scripts => _scripts;

    public async Task<bool> InterceptAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/_debug"))
        {
            if (context.Request.Method == "GET")
            {
                // just read the current state
            }
            else if (context.Request.Method == "POST")
            {
                var segments = context.Request.Path.Value?.Split('/') ?? Array.Empty<string>();

                if (segments.Length < 3)
                {
                    return false;
                }

                // perform action; we read the current state later
                var area = segments[2];

                switch (area)
                {
                    case "event":
                    case "pilet":
                    default:
                        break;
                }
            }
            else
            {
                return false;
            }

            var state = new MfDebugState();
            var content = JsonSerializer.Serialize(state);
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(content));
            return true;
        }

        return false;
    }

    class MfDebugState
    {
        [JsonPropertyName("extensions")]
        public Dictionary<string, string> Extensions => new();

        [JsonPropertyName("dependencies")]
        public Dictionary<string, string> Dependencies => new();

        [JsonPropertyName("pilets")]
        public List<MfPiletInfo> Pilets => new();

        [JsonPropertyName("routes")]
        public List<string> Routes => new();
    }

    class MfPiletInfo
    {
        [JsonPropertyName("name")]
        public string Name => "Example";

        [JsonPropertyName("version")]
        public string Version => "1.0.0";

        [JsonPropertyName("disabled")]
        public bool IsDisabled => false;
    }
}
