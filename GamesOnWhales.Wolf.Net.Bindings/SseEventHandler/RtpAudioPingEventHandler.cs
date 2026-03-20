using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class RtpPingEvent
{
    [JsonPropertyName("client_ip")]
    public required string ClientIp { get; set; }
    [JsonPropertyName("client_port")]
    public required int ClientPort { get; set; }
    [JsonPropertyName("payload")]
    public required ICollection<int> Payload { get; set; }
}

[SseEventHandler]
public partial class RtpAudioPingEventHandler : ISseEventHandler<RtpPingEvent?>
{
    public string EventName => "wolf::core::events::RTPAudioPingEvent";
}