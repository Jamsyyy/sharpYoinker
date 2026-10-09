# sharpYoinker

A Windows desktop youtube video/audio downloader built with **C#**, **Avalonia UI**, and **yt-dlp**.

## Features

- Fetch video metadata from a URL
- Preview video thumbnails and details
- Select available video formats and quality
- Choose a download location
- Track download progress
- Use FFmpeg for media processing

## Getting Started

### Download

Download the latest Windows release from the [GitHub Releases](https://github.com/Jamsyyy/sharpYoinker/releases) page.

1. Download `sharpYoinker-win-x64.zip`.
2. Extract the ZIP file.
3. Run `sharpYoinkerAPP.Desktop.exe`.

The self-contained release should not require a separate .NET installation.

### Build from Source

**Requirements:**
- .NET SDK compatible with the project

Clone the repository:

```bash
git clone https://github.com/Jamsyyy/sharpYoinker.git
cd sharpYoinker
```

Build the Windows desktop project:

```bash
dotnet build ./sharpYoinkerAPP/sharpYoinkerAPP.Desktop/sharpYoinkerAPP.Desktop.csproj
```

**Note:** The bundled executables are not included in the source repository. You must provide the required tools in the expected `tools` directory to use the bundled-tool setup.

## Technologies

- C# / .NET
- Avalonia UI
- yt-dlp
- FFmpeg / FFprobe
- Deno

## License

The original project code is licensed under the MIT License. Third-party tools are subject to their respective licenses. See the included license and third-party notices for details.

## Disclaimer

This project is intended for personal and educational use. Users are responsible for complying with applicable laws, platform terms, and copyright requirements when downloading content.
