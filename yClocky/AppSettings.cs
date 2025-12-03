using System.IO;
using System.Text.Json;
using System.Windows.Media;

namespace yClocky;

public class AppSettings
{
    public string FontFamily { get; set; } = "Segoe UI";
    public string TextColor { get; set; } = "#FFFFFFFF"; // White
    public string BackgroundColor { get; set; } = "#00000000"; // Transparent
    public double Opacity { get; set; } = 1.0;
    public bool ShowDate { get; set; } = true;
    public bool Topmost { get; set; } = true;
    public bool AllowMultipleInstances { get; set; } = false;
    public bool GhostMode { get; set; } = false;
    public bool ExcludeFromCapture { get; set; } = false;
    
    // Window Position
    public double Left { get; set; } = 100;
    public double Top { get; set; } = 100;
}

public static class SettingsManager
{
    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        "yClocky");
    
    private static readonly string SettingsFile = Path.Combine(SettingsFolder, "settings.json");

    public static AppSettings Current { get; private set; } = new AppSettings();

    public static void Load()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    Current = settings;
                }
            }
        }
        catch
        {
            // Fallback to defaults if load fails
            Current = new AppSettings();
        }
    }

    public static event Action? SettingsChanged;

    public static void Save()
    {
        try
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }

            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
            
            SettingsChanged?.Invoke();
        }
        catch
        {
            // Handle save errors (log or ignore)
        }
    }
}
