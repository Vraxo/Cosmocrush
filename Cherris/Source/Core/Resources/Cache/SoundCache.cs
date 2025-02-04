namespace Cherris;

public class SoundCache
{
    private static SoundCache? _instance;
    public static SoundCache Instance => _instance ??= new SoundCache();

    private readonly Dictionary<string, Sound> soundEffects = [];

    private SoundCache() { }

    public Sound Get(string soundKey)
    {
        if (soundEffects.TryGetValue(soundKey, out Sound? soundEffect))
        {
            return soundEffect;
        }

        Sound newSound = new(soundKey);
        soundEffects.Add(soundKey, newSound);

        return newSound;
    }

    public void Dispose()
    {
        foreach (Sound soundEffect in soundEffects.Values)
        {
            soundEffect.Dispose();
        }

        soundEffects.Clear();
    }
}