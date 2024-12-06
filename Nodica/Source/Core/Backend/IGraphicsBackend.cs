namespace Nodica;

public interface IGraphicsBackend
{
    void InitializeWindow(int width, int height, int minWidth, int minHeight, int maxWidth, int maxHeight, string title, string iconPath);
    void SetWindowFlags(bool resizable, bool antiAliasing);
    void SetWindowMinSize(int width, int height);
    void BeginDrawing();
    void ClearBackground(Color color);
    void EndDrawing();
    bool WindowShouldClose();
    bool IsKeyPressed(Key key);
}