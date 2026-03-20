using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class DockerPullImageEndEvent
{
    [JsonPropertyName("image_name")]
    public required string ImageName { get; set; }
    [JsonPropertyName("success")]
    public required bool Success { get; set; }
}

[SseEventHandler]
public partial class DockerPullImageEndEventHandler : ISseEventHandler<DockerPullImageEndEvent?>
{
    public string EventName => "DockerPullImageEndEvent";
}