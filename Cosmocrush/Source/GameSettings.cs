using Cherris;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cosmocrush;

public sealed class GameSettings
{
    private static GameSettings? _instance;
    public static GameSettings Instance => _instance ??= new();

    public SettingsData SettingsData { get; set; } = new();

    private const string path = "Res/GameSettings.yaml";

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
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, yaml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }

        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(path))
            {
                string yaml = File.ReadAllText(path);
                SettingsData = _deserializer.Deserialize<SettingsData>(yaml);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
            SettingsData = new();
        }

        UpdateAudioBusVolumes();
    }

    private void UpdateAudioBusVolumes()
    {
        AudioServerCore.Instance.SetBusVolume("Master", SettingsData.MasterVolume);
        AudioServerCore.Instance.SetBusVolume("Music", SettingsData.MusicVolume);
        AudioServerCore.Instance.SetBusVolume("SFX", SettingsData.SfxVolume);
    }
}