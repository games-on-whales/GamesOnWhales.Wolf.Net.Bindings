using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class ResumeStreamEvent
{
    [JsonPropertyName("session_id")]
    public required string SessionId;
}

[SseEventHandler]
public partial class ResumeStreamEventHandler : ISseEventHandler<ResumeStreamEvent?>
{
    public string EventName => "wolf::core::events::ResumeStreamEvent";
}