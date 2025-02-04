using Raylib_cs;

namespace Cherris;

public class Sound
{
    private Raylib_cs.Sound raylibSound;

    public static implicit operator Raylib_cs.Sound(Sound soundEffect) => soundEffect.raylibSound;

    public Sound(string filePath)
    {
        raylibSound = Raylib.LoadSound(filePath);

        if (raylibSound.FrameCount == 0)
        {
            throw new Exception($"Failed to load sound file: {filePath}");
        }
    }

    public void Play(string bus = "Master")
    {
        AudioServer.PlaySound(this, bus);
    }

    public void Dispose()
    {
        Raylib.UnloadSound(raylibSound);
    }
}