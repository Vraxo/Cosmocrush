using Raylib_cs;

namespace Cherris;

public class Sound
{
    public string Path { get; private set; }
    private Raylib_cs.Sound raylibSound;

    public static implicit operator Raylib_cs.Sound(Sound sound) => sound.raylibSound;

    private Sound(Raylib_cs.Sound sound, string filePath)
    {
        raylibSound = sound;
        Path = filePath;
    }

    public static Sound? Load(string filePath)
    {
        Raylib_cs.Sound sound = Raylib.LoadSound(filePath);

        if (sound.FrameCount == 0) // If loading failed
        {
            return null;
        }

        return new(sound, filePath);
    }

    public void Play(string bus = "Master")
    {
        AudioServer.PlaySound(this, bus);
    }
}