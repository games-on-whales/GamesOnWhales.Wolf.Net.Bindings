using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class SwitchStreamProducerEvent
{
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
    [JsonPropertyName("interpipe_src_id")]
    public required string InterpipeSrcId { get; set; }
}

[SseEventHandler]
public partial class SwitchStreamProducerEventHandler : ISseEventHandler<SwitchStreamProducerEvent?>
{
    public string EventName => "wolf::core::events::SwitchStreamProducerEvents";
}