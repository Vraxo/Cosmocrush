namespace Cherris;

public class SoundCache
{
    private static SoundCache? _instance;
    public static SoundCache Instance => _instance ??= new SoundCache();

    private readonly Dictionary<string, Sound?> soundEffects = [];

    private SoundCache() { }

    public Sound? Get(string soundKey)
    {
        if (soundEffects.TryGetValue(soundKey, out Sound? soundEffect))
        {
            return soundEffect;
        }

        Sound? newSound = Sound.Load(soundKey);

        if (newSound is null)
        {
            Log.Error($"[SoundCache] Could not load sound: {soundKey}");
        }

        soundEffects[soundKey] = newSound;
        return newSound;
    }
}