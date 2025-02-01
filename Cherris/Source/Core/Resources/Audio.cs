using Raylib_cs;

namespace Cherris;

public class Audio
{
    public float Length { get; private set; } = 0.1f;
    private Music raylibAudio;
    private readonly IntPtr sdl2Audio;

    public Audio(string filePath)
    {
        raylibAudio = Raylib.LoadMusicStream(filePath);
        Length = Raylib.GetMusicTimeLength(raylibAudio);
    }

    public static implicit operator Music(Audio audio) => audio.raylibAudio;
}