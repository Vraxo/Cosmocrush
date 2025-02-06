using Raylib_cs;

namespace Cherris;

public class Audio
{
    public float Length { get; private set; } = 0.1f;
    private Music raylibMusic;

    public static implicit operator Music(Audio audio) => audio.raylibMusic;

    private Audio(Music music, float length)
    {
        raylibMusic = music;
        Length = length;
    }

    public static Audio? Load(string filePath)
    {
        Music music = Raylib.LoadMusicStream(filePath);
        float length = Raylib.GetMusicTimeLength(music);

        if (float.IsNaN(length))
        {
            Log.Error($"[Audio] [{filePath} Failed to load.");
            return null;
        }

        return new(music, length);
    }
}