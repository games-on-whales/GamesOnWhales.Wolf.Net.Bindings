using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class DisplayMode
{
    [JsonPropertyName("width")]
    public required int Width { get; set; }
    [JsonPropertyName("height")]
    public required int Height { get; set; }
    [JsonPropertyName("refreshRate")]
    public required int RefreshRate { get; set; }
}

public class VideoSessionEvent
{
    [JsonPropertyName("display_mode")]
    public required DisplayMode DisplayMode { get; set; }
    [JsonPropertyName("gst_pipeline")]
    public required string GstPipeline { get; set; }
    [JsonPropertyName("render_node")]
    public required string RenderNode { get; set; }
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
    [JsonPropertyName("port")]
    public required int Port { get; set; }
    [JsonPropertyName("timeout_ms")]
    public required int TimeoutMs { get; set; }
    [JsonPropertyName("wait_for_ping")]
    public required bool WaitForPing { get; set; }
    [JsonPropertyName("packet_size")]
    public required int PacketSize { get; set; }
    [JsonPropertyName("frames_with_invalid_ref_threshold")]
    public required int FramesWithInvalidRefThreshold { get; set; }
    [JsonPropertyName("fec_percentage")]
    public required int FecPercentage { get; set; }
    [JsonPropertyName("min_required_fec_packets")]
    public required int MinRequiredFecPackets { get; set; }
    [JsonPropertyName("bitrate_kbps")]
    public required int BitrateKbps { get; set; }
    [JsonPropertyName("slices_per_frame")]
    public required int SlicesPerFrame { get; set; }
    [JsonPropertyName("color_range")]
    public required string ColorRange { get; set; }
    [JsonPropertyName("color_space")]
    public required string ColorSpace { get; set; }
    [JsonPropertyName("client_ip")]
    public required string ClientIp { get; set; }
    [JsonPropertyName("rtp_secret_payload")]
    public required ICollection<int> RtpSecretPayload { get; set; }
}

[SseEventHandler]
public partial class VideoSessionEventHandler : ISseEventHandler<VideoSessionEvent?>
{
    public string EventName => "wolf::core::events::VideoSession";
}