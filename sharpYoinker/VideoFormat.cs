public class VideoFormat
{
    public string FormatId { get; set; } = "";
    public string Extension { get; set; } = "";
    public string? Resolution { get; set; }
    public int? Height { get; set; }
    public string? VideoCodec { get; set; }
    public string? AudioCodec { get; set; }
    public long? FileSize { get; set; }
}