namespace Cherris;

public class AudioCache
{
    public static AudioCache? Instance => field ??= new();

    private readonly Dictionary<string, Audio?> _audioStream = [];

    private AudioCache() { }

    public Audio? Get(string filePath)
    {
        if (_audioStream.TryGetValue(filePath, out Audio? audio))
        {
            return audio;
        }

        Audio? newAudio = Audio.Load(filePath);

        if (newAudio == null)
        {
            Log.Error($"[AudioCache] Could not load music: {filePath}");
            return null;
        }

        _audioStream[filePath] = newAudio;
        return newAudio;
    }
}