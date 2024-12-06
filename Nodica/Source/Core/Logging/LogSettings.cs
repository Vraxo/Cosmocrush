using System.Text.Json;

public class LogSettings
{
    public static LogSettings Instance => instance ??= new();
    private static LogSettings? instance;

    public Dictionary<string, bool> Settings { get; private set; }

    private LogSettings()
    {
        string jsonFilePath = "Resources/Nodica/LogSettings.json";
        string jsonString = File.ReadAllText(jsonFilePath);
        Settings = JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonString);
    }

    public bool GetLogCondition(string key)
    {
        return Settings.ContainsKey(key) && Settings[key];
    }
}