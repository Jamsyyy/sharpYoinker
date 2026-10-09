using System;
using System.IO;
using System.Text.Json;

public class SettingsService
{
    private readonly string _settingsPath;

    public SettingsService()
    {
        string appData = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        string appFolder = Path.Combine(
            appData, "sharpYoinker");

        Directory.CreateDirectory(appFolder);

        _settingsPath = Path.Combine(
            appFolder, "settings.json");
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new AppSettings();
        }

        string json = File.ReadAllText(_settingsPath);

        return JsonSerializer.Deserialize<AppSettings>(json)
               ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        string json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_settingsPath, json);
    }
}