using System.Text.Json;

public class VideoMetadataParser
{
    public VideoMetadata Parse(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);

        JsonElement root = document.RootElement;

        List<VideoFormat> formats = new List<VideoFormat>();

        if (root.TryGetProperty("formats", out JsonElement formatsElement))
        {
            foreach (JsonElement format in formatsElement.EnumerateArray())
            {
                string? videoCodec = format.TryGetProperty("vcodec", out JsonElement vcodecElement)
                    ? vcodecElement.GetString()
                    : null;

                if (string.IsNullOrEmpty(videoCodec) || videoCodec == "none")
                {
                    continue;
                }
                VideoFormat videoFormat = new VideoFormat
                {
                    FormatId = format.GetProperty("format_id").GetString() ?? "",
                    Extension = format.GetProperty("ext").GetString() ?? "",
                    Resolution = format.TryGetProperty("resolution", out JsonElement resolution)
                        ? resolution.GetString()
                        : null,
                    Height = format.TryGetProperty("height", out JsonElement height)
                        && height.ValueKind == JsonValueKind.Number
                        ? height.GetInt32()
                        : null,
                    VideoCodec = format.TryGetProperty("vcodec", out JsonElement vcodec)
                        ? vcodec.GetString()
                        : null,
                    AudioCodec = format.TryGetProperty("acodec", out JsonElement acodec)
                        ? acodec.GetString()
                        : null,
                    FileSize = format.TryGetProperty("filesize", out JsonElement filesize)
                        && filesize.ValueKind == JsonValueKind.Number
                        ? filesize.GetInt64()
                        : null
                };
                formats.Add(videoFormat);
            }
        }

        VideoMetadata video = new VideoMetadata
        {
            Title = root.GetProperty("title").GetString() ?? "",
            Uploader = root.TryGetProperty("uploader", out JsonElement uploader)
                ? uploader.GetString()
                : null,
            ThumbnailUrl = root.TryGetProperty("thumbnail", out JsonElement thumbnail)
                ? thumbnail.GetString()
                : null,
            Duration = root.TryGetProperty("duration", out JsonElement duration)
                && duration.ValueKind == JsonValueKind.Number
                    ? duration.GetDouble()
                    : null,

            Formats = formats
        };

        return video;
    }
}