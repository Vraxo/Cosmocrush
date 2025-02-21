using Raylib_cs;

namespace Cherris;

public sealed class RenderServer
{
    public static RenderServer Instance { get; } = new();

    public Camera? Camera;

    private readonly List<DrawCommand> drawCommands = [];

    private RenderServer() { }

    public void Process()
    {
        Raylib.ClearBackground(Color.DarkGray);
        BeginCameraMode();
        ProcessDrawCommands();
        EndCameraMode();
    }

    public void Submit(Action drawAction, int layer)
    {
        drawCommands.Add(new(drawAction, layer));
    }

    // Scissor mode

    public static void BeginScissorMode(Vector2 position, Vector2 size)
    {
        Raylib.BeginScissorMode(
            (int)position.X,
            (int)position.Y,
            (int)size.X,
            (int)size.Y);
    }

    public static void EndScissorMode()
    {
        Raylib.EndScissorMode();
    }

    // Camera

    public void SetCamera(Camera camera)
    {
        Camera = camera;
    }

    private void BeginCameraMode()
    {
        if (Camera is not null)
        {
            Camera2D cam = new()
            {
                Target = Camera.GlobalPosition,
                Offset = VisualServer.WindowSize / 2,
                Zoom = Camera.Zoom
            };

            Raylib.BeginMode2D(cam);
        }
    }

    private void EndCameraMode()
    {
        if (Camera is not null)
        {
            Raylib.EndMode2D();
        }
    }

    // Shader mode

    public static void BeginShaderMode(Shader shader)
    {
        Raylib.BeginShaderMode(shader);
    }

    public static void EndShaderMode()
    {
        Raylib.EndShaderMode();
    }

    public Vector2 GetScreenToWorld(Vector2 position)
    {
        return Raylib.GetScreenToWorld2D(position, Camera);
    }

    private void ProcessDrawCommands()
    {
        // Order the draw commands by layer before invoking them
        foreach (var command in drawCommands.OrderBy(c => c.Layer))
        {
            command.DrawAction.Invoke();
        }

        drawCommands.Clear();
    }

    private class DrawCommand(Action drawAction, int layer)
    {
        public Action DrawAction { get; } = drawAction;
        public int Layer { get; } = layer;
    }
}