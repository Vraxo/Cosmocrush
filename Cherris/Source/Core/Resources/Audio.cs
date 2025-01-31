using Raylib_cs;

namespace Cherris;

public class Audio
{
    public float Length { get; private set; } = 0.1f;
    private Music raylibMusic;
    private Sound raylibSound;

    public Audio(string filePath)
    {
        raylibMusic = Raylib.LoadMusicStream(filePath);
        Length = Raylib.GetMusicTimeLength(raylibMusic);

        raylibSound = Raylib.LoadSound(filePath);
    }

    public static implicit operator Music(Audio audio) => audio.raylibMusic;
    public static implicit operator Sound(Audio audio) => audio.raylibSound;
}