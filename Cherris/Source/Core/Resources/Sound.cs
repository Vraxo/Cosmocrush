using Raylib_cs;

namespace Cherris;

public class Sound(string filePath)
{
    private Raylib_cs.Sound raylibSound = Raylib.LoadSound(filePath);

    public static implicit operator Raylib_cs.Sound(Sound soundEffect)
    {
        return soundEffect.raylibSound;
    }

    public void Play(string bus)
    {
        AudioManager.PlaySound(this, bus);
    }

    public void Dispose()
    {
        Raylib.UnloadSound(raylibSound);
    }
}