using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class IdrRequestEvent
{
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
}

[SseEventHandler]
public partial class IdrRequestEventHandler : ISseEventHandler<IdrRequestEvent?>
{
    public string EventName => "wolf::core::events::IDRRequestEvent";
}