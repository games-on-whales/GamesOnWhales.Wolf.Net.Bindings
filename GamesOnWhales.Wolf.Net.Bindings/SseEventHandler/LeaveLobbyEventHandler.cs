using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class LeaveLobbyEvent
{
    [JsonPropertyName("lobby_id")]
    public required string LobbyId { get; set; }
    [JsonPropertyName("moonlight_session_id")]
    public required string MoonlightSessionId { get; set; }
}

[SseEventHandler]
public partial class LeaveLobbyEventHandler : ISseEventHandler<LeaveLobbyEvent?>
{
    public string EventName => "wolf::core::events::LeaveLobbyEvent";
}