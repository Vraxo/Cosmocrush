namespace Cherris;

public class SoundManager
{
    // Singleton pattern to ensure one instance of SoundManager
    private static SoundManager? _instance;
    public static SoundManager Instance => _instance ??= new SoundManager();

    // Dictionary to store sounds by their key (file path)
    private readonly Dictionary<string, SoundEffect> soundEffects = new();

    // Private constructor to prevent external instantiation
    private SoundManager() { }

    // Get a sound by its file path (key)
    public SoundEffect Get(string soundKey)
    {
        // If sound effect is already loaded, return it
        if (soundEffects.TryGetValue(soundKey, out SoundEffect? soundEffect))
        {
            return soundEffect;
        }

        // Otherwise, load the sound, cache it, and return it
        SoundEffect newSound = new(soundKey);
        soundEffects.Add(soundKey, newSound);
        return newSound;
    }

    // Play a sound by its key
    public void Play(string soundKey)
    {
        SoundEffect soundEffect = Get(soundKey);
        soundEffect.Play();
    }
}
