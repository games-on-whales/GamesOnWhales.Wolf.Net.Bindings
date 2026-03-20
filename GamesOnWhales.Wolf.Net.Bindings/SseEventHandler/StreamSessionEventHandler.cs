using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class ClientSettings
{
    [JsonPropertyName("run_uid")]
    public required int RunUid { get; set; }
    [JsonPropertyName("run_gid")]
    public required int RunGid { get; set; }
    [JsonPropertyName("controllers_override")]
    public required ICollection<Controllers_override2> ControllersOverride { get; set; }
    [JsonPropertyName("mouse_acceleration")]
    public required double MouseAcceleration { get; set; }
    [JsonPropertyName("v_scroll_acceleration")]
    public required double VScrollAcceleration { get; set; }
    [JsonPropertyName("h_scroll_acceleration")]
    public required double HScrollAcceleration { get; set; }
}

public class StreamSessionEvent
{
    [JsonPropertyName("client_ip")]
    public required string ClientIp { get; set; }
    [JsonPropertyName("aes_key")]
    public required string AesKey { get; set; }
    [JsonPropertyName("aes_iv")]
    public required string AesIv { get; set; }
    [JsonPropertyName("rtsp_fake_ip")]
    public required string RtspFakeIp { get; set; }
    [JsonPropertyName("video_width")]
    public required int VideoWidth { get; set; }
    [JsonPropertyName("video_height")]
    public required int VideoHeight { get; set; }
    [JsonPropertyName("video_refresh_rate")]
    public required int VideoRefreshRate { get; set; }
    [JsonPropertyName("app_id")]
    public required string AppId { get; set; }
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }
    [JsonPropertyName("client_settings")]
    public required ClientSettings ClientSettings { get; set; }
}

[SseEventHandler]
public partial class StreamSessionEventHandler : ISseEventHandler<StreamSessionEvent?>
{
    public string EventName => "wolf::core::events::StreamSession";
}