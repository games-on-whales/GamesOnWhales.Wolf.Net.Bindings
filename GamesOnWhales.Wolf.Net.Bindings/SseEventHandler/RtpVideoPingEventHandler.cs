namespace GamesOnWhales.SSE;

[SseEventHandler]
public partial class RtpVideoPingEventHandler : ISseEventHandler<RtpPingEvent?>
{
    public string EventName => "wolf::core::events::RTPVideoPingEvent";
}