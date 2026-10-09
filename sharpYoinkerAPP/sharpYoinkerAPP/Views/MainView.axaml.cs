using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using sharpYoinkerAPP.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Globalization;
using System.Text.RegularExpressions;

namespace sharpYoinkerAPP.Views;

public partial class MainView : ContentPage
{
    private static readonly HttpClient HttpClient = new();
    private string _outputFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Downloads",
        "sharpYoinker");

    public MainView()
    {
        InitializeComponent();
        var settingsService = new SettingsService();
        var settings = settingsService.Load();

        if (!string.IsNullOrWhiteSpace(settings.DownloadFolder) &&
            Directory.Exists(settings.DownloadFolder))
        {
            _outputFolder = settings.DownloadFolder;
        }

        OutputPathInput.Text = _outputFolder;

        FetchButton.Click += FetchButton_Click;
        DownloadButton.Click += DownloadButton_Click;
        BrowseButton.Click += BrowseButton_Click;
        FormatSelector.SelectionChanged += FormatSelector_SelectionChanged;
    }

    private async void FetchButton_Click(object? sender, RoutedEventArgs e)
    {
        string url = UrlInput.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(url))
        {
            StatusText.Text = "Please enter a video URL.";
            return;
        }

        FetchButton.IsEnabled = false;
        StatusText.Text = "Fetching video information...";
        TitleText.Text = "";
        UploaderText.Text = "";
        DurationText.Text = "";
        ResolutionText.Text = "";
        ThumbnailImage.IsVisible = false;

        try
        {
            var ytdlpPath = Path.Combine(
                AppContext.BaseDirectory,
                "yt-dlp.exe"
            );

            var runner = new YtDlpRunner(ytdlpPath);

            var result = await runner.RunAsync(new[]
            {
                "--dump-single-json",
                "--skip-download",
                url
            });

            if (result.ExitCode != 0)
                throw new Exception(result.StandardError);

            var parser = new VideoMetadataParser();
            var video = parser.Parse(result.StandardOutput);

         
            TitleText.Text = video.Title;
            UploaderText.Text = $"Uploader: {video.Uploader ?? "Unknown"}";

            DurationText.Text = video.Duration.HasValue
                ? $"Duration: {TimeSpan.FromSeconds(video.Duration.Value):mm\\:ss}"
                : "Duration: Unknown";

            var resolutions = video.GetAvailableResolutions();

            QualitySelector.ItemsSource = resolutions
                .Select(height => $"{height}p")
                .ToList();

            QualitySelector.SelectedIndex = resolutions.Count > 0 ? 0 : -1;

            ResolutionText.Text = resolutions.Count > 0
                ? $"Available resolutions: {string.Join(", ", resolutions.Select(height => $"{height}p"))}"
                : "No video resolutions found.";

            if (!string.IsNullOrWhiteSpace(video.ThumbnailUrl))
            {
                try
                {
                    byte[] imageBytes =
                        await HttpClient.GetByteArrayAsync(video.ThumbnailUrl);

                    using var stream = new MemoryStream(imageBytes);
                    ThumbnailImage.Source = new Bitmap(stream);
                    ThumbnailImage.IsVisible = true;
                }
                catch
                {
                    StatusText.Text =
                        "Video information loaded, but the thumbnail failed to load.";
                }
            }

            if (StatusText.Text == "Fetching video information...")
                StatusText.Text = "Video information loaded!";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            FetchButton.IsEnabled = true;
        }
    }
    private async void DownloadButton_Click(object? sender, RoutedEventArgs e)
    {
        string url = UrlInput.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(url))
        {
            StatusText.Text = "Please enter a video URL first.";
            return;
        }

        bool isMp3 = FormatSelector.SelectedIndex == 1;

        int quality = 1080;
        if (QualitySelector.SelectedItem is string selectedQuality)
        {
            int.TryParse(
                selectedQuality.Replace("p", ""),
                out quality);
        }

        DownloadButton.IsEnabled = false;
        FetchButton.IsEnabled = false;
        StatusText.Text = "Preparing download...";
        DownloadProgress.Value = 0;
        DownloadProgress.IsVisible = true;

        try
        {
            Directory.CreateDirectory(_outputFolder);

            var runner = new YtDlpRunner(@"D:\yt-dlp.exe");
            var service = new DownloadService(runner);

            string outputTemplate = Path.Combine(
                _outputFolder,
                "%(title)s.%(ext)s");


            var progress = new Progress<string>(line =>
            {
                System.IO.File.AppendAllText(
                    @"D:\yt-progress.txt",
                    line + Environment.NewLine);

                var match = Regex.Match(line, @"\[download\]\s+([\d.]+)%");

                if (match.Success &&
                    double.TryParse(
                        match.Groups[1].Value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double percent))
                {
                    DownloadProgress.Value = percent;

                    StatusText.Text = $"Downloading... {percent:0.0}%";
                }
            });
            var result = isMp3
            ? await service.DownloadMp3Async(url, outputTemplate, progress)
            : await service.DownloadMp4Async(url, outputTemplate, quality, progress);

            if (result.ExitCode != 0)
            {
                StatusText.Text =
                    $"Download failed:\n{result.StandardError}";
                return;
            }

            DownloadProgress.Value = 100;
            StatusText.Text = $"Download complete! Saved to:\n{_outputFolder}";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            DownloadButton.IsEnabled = true;
            FetchButton.IsEnabled = true;
        }

    }
    private void FormatSelector_SelectionChanged(
    object? sender,
    SelectionChangedEventArgs e)
    {
        QualityPanel.IsVisible = FormatSelector.SelectedIndex == 0;
    }

    private async void BrowseButton_Click(
    object? sender,
    RoutedEventArgs e)
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;

        if (storage is null)
        {
            StatusText.Text = "Folder picker is unavailable.";
            return;
        }

        var folders = await storage.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Choose download folder",
                AllowMultiple = false
            });

        if (folders.Count > 0)
        {
            string? selectedPath = folders[0].Path.LocalPath;

            if (!string.IsNullOrWhiteSpace(selectedPath))
            {
                _outputFolder = selectedPath;
                OutputPathInput.Text = _outputFolder;
                var settingsService = new SettingsService();
                var settings = settingsService.Load();
                settings.DownloadFolder = _outputFolder;
                settingsService.Save(settings);
                StatusText.Text = "Download folder updated.";
            }
        }
    }
}