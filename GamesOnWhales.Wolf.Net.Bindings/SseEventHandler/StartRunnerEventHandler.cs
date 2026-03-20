using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class Runner
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("image")]
    public required string Image { get; set; }
    [JsonPropertyName("mounts")]
    public required ICollection<string> Mounts { get; set; }
    [JsonPropertyName("env")]
    public required ICollection<string> Env { get; set; }
    [JsonPropertyName("devices")]
    public required ICollection<string> Devices { get; set; }
    [JsonPropertyName("ports")]
    public required ICollection<string> Ports { get; set; }
    [JsonPropertyName("base_create_json")]
    public required string BaseCreateJson { get; set; }
    [JsonPropertyName("session_id")]
    public string? SessionId { get; set; }
}

public class StartRunnerEvent
{
    [JsonPropertyName("stop_stream_when_over")]
    public required bool StopStreamWhenOver { get; set; }
    [JsonPropertyName("runner")]
    public required Runner Runner { get; set; }
}

[SseEventHandler]
public partial class StartRunnerEventHandler : ISseEventHandler<StartRunnerEvent?>
{
    public string EventName => "wolf::core::events::StartRunner";
}