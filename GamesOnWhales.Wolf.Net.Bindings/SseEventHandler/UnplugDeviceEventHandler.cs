namespace GamesOnWhales.SSE;

[SseEventHandler]
public partial class UnplugDeviceEventHandler : ISseEventHandler<string>
{
    public string EventName => "wolf::core::events::UnPlugDeviceEvent";
}