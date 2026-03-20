using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class PauseStreamEvent
{
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
}

[SseEventHandler]
public partial class PauseStreamEventHandler : ISseEventHandler<PauseStreamEvent?>
{
    public string EventName => "wolf::core::events::PauseStreamEvent";
}