using Raylib_cs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class AudioManagerCore
{
    private static AudioManagerCore? _instance;
    public static AudioManagerCore Instance => _instance ??= new();

    private readonly Dictionary<string, float> buses = [];
    private const string busesPath = "Res/Cherris/AudioBuses.yaml";

    public delegate void BusVolumeChangedEventHandler(string bus, float volume);
    public event BusVolumeChangedEventHandler? VolumeChanged;

    private AudioManagerCore()
    {
        LoadBuses();
    }

    // Playing

    public void PlaySound(SoundEffect sound, string bus = "Master")
    {
        Raylib.PlaySound(sound);
        Raylib.SetSoundVolume(sound, GetBusVolume(bus));
    }

    // Buses

    public void SetBusVolume(string bus, float volume)
    {
        if (!buses.ContainsKey(bus))
        {
            throw new Exception($"Bus '{bus}' does not exist.");
        }

        buses[bus] = volume;
        VolumeChanged?.Invoke(bus, volume);
    }

    public float GetBusVolume(string bus)
    {
        if (!buses.TryGetValue(bus, out float value))
        {
            throw new Exception($"Bus '{bus}' does not exist.");
        }

        return value;
    }

    public void AddBus(string name, float volume = 1)
    {
        if (buses.ContainsKey(name))
        {
            throw new Exception($"Bus '{name}' already exists.");
        }

        buses.Add(name, volume);
    }

    private void LoadBuses()
    {
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, busesPath);
        string directory = Path.GetDirectoryName(fullPath)!;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(fullPath))
        {
            Dictionary<string, float> defaultBuses = new() { { "Master", 1.0f } };
            SaveBusesToYaml(fullPath, defaultBuses);
        }

        string yamlContent = File.ReadAllText(fullPath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var loadedBuses = deserializer.Deserialize<Dictionary<string, float>>(yamlContent);

        foreach (KeyValuePair<string, float> kvp in loadedBuses)
        {
            buses[kvp.Key] = kvp.Value;
        }
    }

    private static void SaveBusesToYaml(string path, Dictionary<string, float> busesToSave)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(busesToSave);
        File.WriteAllText(path, yaml);
    }
}