using Raylib_cs;

namespace Cherris;

public sealed class AudioServer
{
    public static AudioServer Instance { get; } = new();

    private const string busesPath = "Res/Cherris/AudioBuses.yaml";

    public delegate void BusVolumeEvent(string bus, float volume);
    public event BusVolumeEvent? VolumeChanged;

    private readonly Dictionary<string, float> buses = [];

    // Main

    private AudioServer()
    {
        LoadBuses();
    }
    
    // Play

    public void PlaySound(Sound sound, string bus = "Master")
    {
        Raylib_cs.Sound clone = Raylib.LoadSoundAlias(sound);
        Raylib.PlaySound(clone);
        Raylib.SetSoundVolume(clone, GetBusVolume(bus));
    }

    // Buses

    public void SetBusVolume(string bus, float volume)
    {
        if (!buses.ContainsKey(bus))
        {
            Log.Error($"[AudioServer]: Cannot set bus volume. Bus '{bus}' does not exist.");
            return;
        }

        buses[bus] = volume;
        VolumeChanged?.Invoke(bus, volume);
    }

    public float GetBusVolume(string bus)
    {
        if (!buses.TryGetValue(bus, out float value))
        {
            Log.Error($"[AudioServer]: Cannot get bus volume. Bus '{bus}' does not exist.");
        }

        return value;
    }

    public void AddBus(string name, float volume = 1)
    {
        if (buses.ContainsKey(name))
        {
            Log.Error($"[AudioServer]: Bus '{name}' aleady exists");
            return;
        }

        buses.Add(name, volume);
    }

    public bool BusExists(string name)
    {
        return buses.ContainsKey(name);
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
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var loadedBuses = deserializer.Deserialize<Dictionary<string, float>>(yamlContent);

        foreach (KeyValuePair<string, float> kvp in loadedBuses)
        {
            buses[kvp.Key] = kvp.Value;
        }
    }

    private static void SaveBusesToYaml(string path, Dictionary<string, float> busesToSave)
    {
        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(busesToSave);
        File.WriteAllText(path, yaml);
    }
}