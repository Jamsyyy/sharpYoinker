using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;


//debugging lolllll 
YtDlpRunner runner = new YtDlpRunner(@"D:\yt-dlp.exe");
DownloadService service = new DownloadService(runner);

//YtDlpResult result = await service.DownloadMp4Async(
//    "https://www.youtube.com/watch?v=dZzD8kSPYRQ",
//    @"D:\Downloads\%(title)s.%(ext)s",
//    9999
//);

//Console.WriteLine($"Exit code: {result.ExitCode}");
//if (result.ExitCode != 0)
//{
//    if (result.StandardError.Contains("Requested format is not available"))
//    {
//        Console.WriteLine("The selected resolution is not available.");
//    }
//    else
//    {
//        Console.WriteLine("Download failed. Please try again.");
//    }
//}
//else
//{
//    Console.WriteLine("Download completed!");
//}
//Console.WriteLine(result.StandardOutput);
//Console.WriteLine(result.StandardError);
VideoMetadataParser parser = new VideoMetadataParser();


YtDlpResult result = await runner.RunAsync(new[]
{
    "--dump-single-json",
    "--skip-download",
    "https://www.youtube.com/watch?v=2hPuRQz6IlM"
});

if (result.ExitCode != 0)
{
    Console.WriteLine("Failed to retrieve metadata.");
    Console.WriteLine(result.StandardError);
    return;
}

VideoMetadata video = parser.Parse(result.StandardOutput);


Console.WriteLine($"Title: {video.Title}");
Console.WriteLine($"Uploader: {video.Uploader}");
Console.WriteLine($"Thumbnail: {video.ThumbnailUrl}");
Console.WriteLine($"Duration: {video.Duration} seconds");
Console.WriteLine($"Formats found: {video.Formats.Count}");
List<int> resolutions = video.GetAvailableResolutions();

Console.WriteLine("Available resolutions:");

foreach (int height in resolutions)
{
    Console.WriteLine($"{height}p");
}