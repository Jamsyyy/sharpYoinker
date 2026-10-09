using System.Collections.Generic;
using System.Threading.Tasks;

public class DownloadService
{
    private readonly YtDlpRunner _runner;

    public DownloadService(YtDlpRunner runner)
    {
        _runner = runner;
    }

    public async Task<YtDlpResult> DownloadMp3Async(
        string url,
        string outputPath,
        IProgress<string>? progress = null)
    {
        var arguments = new List<string>
        {
            "--js-runtimes",
            "node",
            "--newline",
            "--progress",
            "-x",
            "--audio-format", "mp3",
            "-o", outputPath,
            url
        };

        return await _runner.RunAsync(arguments, progress);
    }

    public async Task<YtDlpResult> DownloadMp4Async(
        string url,
        string outputPath,
        int quality,
        IProgress<string>? progress = null)
    {
        var arguments = new List<string>
        {
            "--js-runtimes",
            "node",
            "--newline",
            "--progress",
            "-f", $"bestvideo[height={quality}]+bestaudio/best[height={quality}]",
            "--merge-output-format", "mp4",
            "-o", outputPath,
            url
        };

        return await _runner.RunAsync(arguments, progress);
    }
}