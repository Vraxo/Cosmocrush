using Raylib_cs;

namespace Nodica;

public static class Time
{
    public static float Delta => Raylib.GetFrameTime();
    public static float Elapsed => (float)Raylib.GetTime();
}