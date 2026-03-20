using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class DockerPullImageStartEvent
{
    [JsonPropertyName("image_name")]
    public required bool ImageName { get; set; }
}

[SseEventHandler]
public partial class DockerPullImageStartEventHandler : ISseEventHandler<DockerPullImageStartEvent?>
{
    public string EventName => "DockerPullImageStartEvent";
}