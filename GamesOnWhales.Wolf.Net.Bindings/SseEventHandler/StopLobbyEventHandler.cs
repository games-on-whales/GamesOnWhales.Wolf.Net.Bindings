using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class StopLobbyEvent
{
    [JsonPropertyName("lobby_id")]
    public required string LobbyId { get; set; }
}

[SseEventHandler]
public partial class StopLobbyEventHandler : ISseEventHandler<StopLobbyEvent?>
{
    public string EventName => "wolf::core::events::StopLobbyEvent";
}