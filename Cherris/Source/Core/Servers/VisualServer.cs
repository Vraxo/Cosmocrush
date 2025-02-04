using Raylib_cs;

namespace Cherris;

public static class VisualServer
{
    public static Vector2 OriginalWindowSize = Vector2.Zero;
    public static Vector2 PreviousWindowSize = Vector2.Zero;

    public static Vector2 WindowSize
    {
        get => new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        set
        {
            Raylib.SetWindowSize((int)value.X, (int)value.Y);
        }
    }

    public static Vector2 Size
    {
        get
        {
            int currentMonitor = Raylib.GetCurrentMonitor();
            int width = Raylib.GetMonitorWidth(currentMonitor);
            int height = Raylib.GetMonitorHeight(currentMonitor);

            return new(width, height);
        }
    }

    public static bool Fullscreen => Raylib.IsWindowFullscreen();

    public static void ToggleFullscreen()
    {
        Raylib.ToggleFullscreen();
    }
}