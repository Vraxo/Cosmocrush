namespace Cherris;

public class AudioStreamCache
{
    public static AudioStreamCache? Instance => field ??= new();

    private readonly Dictionary<string, AudioStream?> _audioStream = [];

    private AudioStreamCache() { }

    public AudioStream? Get(string filePath)
    {
        if (_audioStream.TryGetValue(filePath, out AudioStream? audio))
        {
            return audio;
        }

        AudioStream? newAudio = AudioStream.Load(filePath);

        if (newAudio == null)
        {
            Log.Error($"[AudioStreamCache] Could not load music: {filePath}");
            return null;
        }

        _audioStream[filePath] = newAudio;
        return newAudio;
    }
}