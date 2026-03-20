using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class PairSignalEvent
{
    [JsonPropertyName("client_ip")]
    public required string ClientIp { get; set; }
    [JsonPropertyName("host_ip")]
    public required string HostIp { get; set; }
}

[SseEventHandler]
public partial class PairSignalEventHandler : ISseEventHandler<PairSignalEvent?>
{
    public string EventName => "wolf::core::events::PairSignal";
}