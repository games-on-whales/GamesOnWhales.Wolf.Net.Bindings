using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class AudioSettings
{
    [JsonPropertyName("channel_count")]
    public required int ChannelCount { get; set; }
}

public class VideoSettings
{
    [JsonPropertyName("width")]
    public required int Width { get; set; }
    [JsonPropertyName("height")]
    public required int Height { get; set; }
    [JsonPropertyName("refresh_rate")]
    public required int RefreshRate { get; set; }
    [JsonPropertyName("wayland_render_node")]
    public required string WaylandRenderNode { get; set; }
    [JsonPropertyName("runner_render_node")]
    public required string RunnerRenderNode { get; set; }
    [JsonPropertyName("video_producer_buffer_caps")]
    public required string VideoProducerBufferCaps { get; set; }
}

public class CreateLobbyEvent
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("profile_id")]
    public required string ProfileId { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("icon_png_path")]
    public required string IconPngPath { get; set; }
    [JsonPropertyName("multi_user")]
    public required bool MultiUser { get; set; }
    [JsonPropertyName("stop_when_everyone_leaves")]
    public required bool StopWhenEveryoneLeaves { get; set; }
    [JsonPropertyName("runner_state_folder")]
    public required string RunnerStateFolder { get; set; }
    [JsonPropertyName("video_settings")]
    public required VideoSettings VideoSettings { get; set; }
    [JsonPropertyName("audio_settings")]
    public required AudioSettings AudioSettings { get; set; }
    [JsonPropertyName("client_settings")]
    public required ClientSettings ClientSettings { get; set; }
    [JsonPropertyName("runner")]
    public required Runner Runner { get; set; }
}

[SseEventHandler]
public partial class CreateLobbyEventHandler : ISseEventHandler<CreateLobbyEvent?>
{
    public string EventName => "wolf::core::events::CreateLobbyEvent";
}