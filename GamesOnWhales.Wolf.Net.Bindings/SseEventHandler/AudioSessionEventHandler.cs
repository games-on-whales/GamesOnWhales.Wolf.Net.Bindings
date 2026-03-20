using System.Text.Json.Serialization;

namespace GamesOnWhales.SSE;

public class AudioMode
{
    [JsonPropertyName("channels")]
    public required int Channels { get; set; }
    [JsonPropertyName("streams")]
    public required int Streams { get; set; }
    [JsonPropertyName("coupled_streams")]
    public required int CoupledStreams { get; set; }
    [JsonPropertyName("speakers")]
    public required ICollection<string> Speakers { get; set; }
    [JsonPropertyName("bitrate")]
    public required int Bitrate { get; set; }
    [JsonPropertyName("sample_rate")]
    public required int SampleRate { get; set; }
}

public class AudioSessionEvent
{
    [JsonPropertyName("gst_pipeline")]
    public required string GstPipeline { get; set; }
    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }
    [JsonPropertyName("encrypt_audio")]
    public required bool EncryptAudio { get; set; }
    [JsonPropertyName("aes_key")]
    public required string AesKey { get; set; }
    [JsonPropertyName("aes_iv")]
    public required string AesIv { get; set; }
    [JsonPropertyName("port")]
    public required int Port { get; set; }
    [JsonPropertyName("wait_for_ping")]
    public required bool WaitForPing { get; set; }
    [JsonPropertyName("client_ip")]
    public required string ClientIp { get; set; }
    [JsonPropertyName("rtp_secret_payload")]
    public required ICollection<int> RtpSecretPayload { get; set; }
    [JsonPropertyName("packet_duration")]
    public required int PacketDuration { get; set; }
    [JsonPropertyName("audio_mode")]
    public required AudioMode AudioMode { get; set; }
}

[SseEventHandler]
public partial class AudioSessionEventHandler : ISseEventHandler<AudioSessionEvent?>
{
    public string EventName => "wolf::core::events::AudioSession";
}