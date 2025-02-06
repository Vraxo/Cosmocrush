namespace Cherris;

public class AudioCache
{
    private static AudioCache? _instance;
    public static AudioCache Instance => _instance ??= new AudioCache();

    private readonly Dictionary<string, Audio?> _audioTracks = [];

    private AudioCache() { }

    public Audio? Get(string filePath)
    {
        if (_audioTracks.TryGetValue(filePath, out Audio? audio))
        {
            return audio;
        }

        Audio? newAudio = Audio.Load(filePath);
        if (newAudio == null)
        {
            Log.Error($"[AudioCache] Could not load music: {filePath}");
        }

        _audioTracks[filePath] = newAudio; // Cache even failed loads
        return newAudio;
    }
}