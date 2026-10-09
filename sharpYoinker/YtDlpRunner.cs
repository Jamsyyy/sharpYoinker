
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public class YtDlpRunner
{
    private readonly string _executablePath;

    public YtDlpRunner(string executablePath)
    {
        _executablePath = executablePath;
    }

    public async Task<YtDlpResult> RunAsync(
        IEnumerable<string> arguments,
        IProgress<string>? progress = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }
        System.IO.File.WriteAllText(
            $@"D:\yt-debug-{DateTime.Now:yyyyMMdd-HHmmss-fff}.txt",
            $"EXE: {startInfo.FileName}\n" +
            $"ARGS:\n{string.Join("\n", startInfo.ArgumentList)}");

        using var process = new Process
        {
            StartInfo = startInfo
        };
        System.Diagnostics.Debug.WriteLine(
            $"YT-DLP EXE: {startInfo.FileName}");

        System.Diagnostics.Debug.WriteLine(
            $"YT-DLP ARGS: {string.Join(" | ", startInfo.ArgumentList)}");
        process.Start();

        var output = new StringBuilder();
        var error = new StringBuilder();

        Task outputTask = ReadStreamAsync(
            process.StandardOutput, output, progress);

        Task errorTask = ReadStreamAsync(
            process.StandardError, error, progress);

        await process.WaitForExitAsync();
        await Task.WhenAll(outputTask, errorTask);

        return new YtDlpResult(
            process.ExitCode,
            output.ToString(),
            error.ToString());
    }

    private static async Task ReadStreamAsync(
        System.IO.StreamReader reader,
        StringBuilder destination,
        IProgress<string>? progress)
    {
        var message = new StringBuilder();
        char[] buffer = new char[1024];
        int count;

        while ((count = await reader.ReadAsync(
            buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < count; i++)
            {
                char c = buffer[i];
                destination.Append(c);

                if (c == '\r' || c == '\n')
                {
                    if (message.Length > 0)
                    {
                        progress?.Report(message.ToString());
                        message.Clear();
                    }
                }
                else
                {
                    message.Append(c);
                }
            }
        }

        if (message.Length > 0)
        {
            progress?.Report(message.ToString());
        }
    }
}
