using Raylib_cs;

namespace Cherris;

public class AudioStream
{
    public float Length { get; private set; } = 0.1f;
    public string Path { get; private set; }
    
    private Music raylibMusic;

    public static implicit operator Music(AudioStream audio) => audio.raylibMusic;

    private AudioStream(Music music, float length, string path)
    {
        raylibMusic = music;
        Length = length;
        Path = path;
    }

    public static AudioStream? Load(string filePath)
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