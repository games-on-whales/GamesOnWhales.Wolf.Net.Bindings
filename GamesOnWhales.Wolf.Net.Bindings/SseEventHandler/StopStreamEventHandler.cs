using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class StopStreamEvent
{
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
}

[SseEventHandler]
public partial class StopStreamEventHandler : ISseEventHandler<StopStreamEvent?>
{
    public string EventName => "wolf::core::events::StopStreamEvent";
}