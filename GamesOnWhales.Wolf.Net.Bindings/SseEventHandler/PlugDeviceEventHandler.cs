namespace GamesOnWhales.SSE;

// public class PlugDeviceEvent
// {
//     
// }

[SseEventHandler]
public partial class PlugDeviceEventHandler : ISseEventHandler<string>
{
    public string EventName => "wolf::core::events::PlugDeviceEvent";
}