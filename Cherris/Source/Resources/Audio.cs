using Raylib_cs;

namespace Cherris;

public class Audio
{
    public float Length { get; private set; } = 0.1f;
    public string Path { get; private set; }
    
    private Music raylibMusic;

    public static implicit operator Music(Audio audio) => audio.raylibMusic;

    private Audio(Music music, float length, string path)
    {
        raylibMusic = music;
        Length = length;
        Path = path;
    }

    public static Audio? Load(string filePath)
    {
        Music music = Raylib.LoadMusicStream(filePath);
        float length = Raylib.GetMusicTimeLength(music);

        if (float.IsNaN(length))
        {
            return null;
        }

        return new(music, length, filePath);
    }
}