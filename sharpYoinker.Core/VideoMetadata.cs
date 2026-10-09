using System.Text.Json;

public class VideoMetadata
{
    public string Title { get; set; } = "";
    public string? Uploader { get; set; }
    public string? ThumbnailUrl { get; set; }
    public double? Duration { get; set; }

    public List<VideoFormat> Formats { get; set; } = new();

    public List<int> GetAvailableResolutions()
    {
        return Formats
            .Where(format => format.Height.HasValue)
            .Select(format => format.Height!.Value)
            .Distinct()
            .OrderByDescending(height => height)
            .ToList();
    }
}

