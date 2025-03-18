using Raylib_cs;

namespace Cherris;

public sealed class Time
{
    public static float Delta => TimeServer.Instance.Delta;
    public static float Elapsed => TimeServer.Elapsed;
}

public class TimeServer
{
    private static TimeServer? _instance;
    public static TimeServer Instance => _instance ??= new();

    public float Delta { get; private set; } = 0;

    public static float Elapsed => (float)Raylib.GetTime();

    public void Process()
    {
        Delta = Raylib.GetFrameTime();
    }
}