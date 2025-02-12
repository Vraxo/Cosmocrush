using Raylib_cs;

namespace Cherris;

public class TimeServer
{
    public static TimeServer Instance { get; } = new();

    public static float Delta => Instance._delta;
    public static float Elapsed => Instance._elapsed;

    private float _delta = 0;
    private float _elapsed => (float)Raylib.GetTime();

    public void Process()
    {
        _delta = Raylib.GetFrameTime();
    }
}