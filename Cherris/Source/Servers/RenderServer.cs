using Raylib_cs;

namespace Cherris;

public sealed class RenderServer
{
    private static RenderServer? _instance;
    public static RenderServer Instance => _instance ??= new();

    public Camera Camera;
    private readonly List<DrawCommand> drawCommands = [];

    private RenderServer() { }

    public void Process()
    {
        //Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DarkGray);
        BeginCameraMode();
        //AppServer.Instance.RootNode?.Process();
        ProcessDrawCommands();
        EndCameraMode();
        //Raylib.EndDrawing();
    }

    public void Submit(DrawCommand drawCommand)
    {
        drawCommands.Add(drawCommand);
    }

    public void SetCamera(Camera camera)
    {
        Camera = camera;
    }

    public Vector2 GetScreenToWorld(Vector2 position)
    {
        return Raylib.GetScreenToWorld2D(position, Camera);
    }

    private void ProcessDrawCommands()
    {
        foreach (DrawCommand command in drawCommands.OrderBy(c => c.Layer))
        {
            command.Draw();
        }

        drawCommands.Clear();
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
}