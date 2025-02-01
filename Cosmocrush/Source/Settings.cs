using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cosmocrush;

public sealed class Settings
{
    private static Settings? _instance;
    public static Settings Instance => _instance ??= new();

    public SettingsData SettingsData { get; set; } = new();

    private const string path = "Res/Settings.yaml"; // Make sure this path is correct

    private static readonly ISerializer _serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public void Save()
    {
        try
        {
            string yaml = _serializer.Serialize(SettingsData);
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, yaml);
            Console.WriteLine($"Settings saved to: {path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(path))
            {
                string yaml = File.ReadAllText(path);
                SettingsData = _deserializer.Deserialize<SettingsData>(yaml);
                Console.WriteLine($"Settings loaded from: {path}");
            }
            else
            {
                Console.WriteLine("Settings file not found. Using default settings.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
            // Optionally reset to default settings if loading fails
            SettingsData = new SettingsData();
        }
    }
}