using Raylib_cs;

namespace Nodica;

public class Audio
{
    public float Length { get; private set; } = 0;
    private Music audio;

    public Audio(string filePath)
    {
        audio = Raylib.LoadMusicStream(filePath);
        Length = Raylib.GetMusicTimeLength(audio);
    }

    public static implicit operator Music(Audio audio) => audio.audio;
}