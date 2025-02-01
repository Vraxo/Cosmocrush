using Raylib_cs;

namespace Cherris;

public class SoundEffect
{
    private Sound raylibSound;

    public SoundEffect(string filePath)
    {
        raylibSound = Raylib.LoadSound(filePath);
    }

    public static implicit operator Sound(SoundEffect soundEffect)
    {
        return soundEffect.raylibSound;
    }

    public void Play()
    {
        Raylib.PlaySound(raylibSound);
    }
}