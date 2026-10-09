public class YtDlpResult
{
    public int ExitCode { get; }
    public string StandardOutput { get; }
    public string StandardError { get; }

    public YtDlpResult(
        int exitCode,
        string standardOutput,
        string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }
}