using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class JoinLobbyEvent
{
    [JsonPropertyName("lobby_id")]
    public required string LobbyId { get; set; }
    [JsonPropertyName("moonlight_session_id")]
    public required string MoonlightSessionId { get; set; }
}

[SseEventHandler]
public partial class JoinLobbyEventHandler : ISseEventHandler<JoinLobbyEvent?>
{
    public string EventName => "wolf::core::events::JoinLobbyEvent";
}