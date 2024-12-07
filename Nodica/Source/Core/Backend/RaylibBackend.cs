namespace Nodica.Backends;

using Raylib_cs;

public sealed class RaylibBackend : IGraphicsBackend
{
    public void InitializeWindow(int width, int height, int minWidth, int minHeight, int maxWidth, int maxHeight, string title, string iconPath)
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);
        Raylib.SetConfigFlags(ConfigFlags.VSyncHint | ConfigFlags.HighDpiWindow | ConfigFlags.AlwaysRunWindow);
        Raylib.InitWindow(width, height, title);
        Raylib.SetWindowMinSize(minWidth, minHeight);
        Raylib.SetWindowMaxSize(maxWidth, maxHeight);
        Raylib.SetWindowIcon(Raylib.LoadImage(iconPath));
        Raylib.InitAudioDevice();
    }

    public void SetWindowFlags(bool resizable, bool antiAliasing)
    {
        ConfigFlags flags =
            ConfigFlags.VSyncHint |
            ConfigFlags.HighDpiWindow |
            ConfigFlags.AlwaysRunWindow;

        if (resizable)
        {
            flags |= ConfigFlags.ResizableWindow;
        }

        if (antiAliasing)
        {
            flags |= ConfigFlags.Msaa4xHint;
        }

        Raylib.SetConfigFlags(flags);
    }

    public void SetWindowMinSize(int width, int height)
    {
        Raylib.SetWindowMinSize(width, height);
    }

    public void BeginDrawing()
    {
        Raylib.BeginDrawing();
    }

    public void ClearBackground(Color color)
    {
        Raylib.ClearBackground(color);
    }

    public void EndDrawing()
    {
        Raylib.EndDrawing();
    }

    public bool WindowShouldClose()
    {
        return Raylib.WindowShouldClose();
    }

    public bool IsKeyPressed(Key key)
    {
        return Raylib.IsKeyPressed((KeyboardKey)key);
    }
}