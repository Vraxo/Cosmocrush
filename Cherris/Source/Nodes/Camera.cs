using Raylib_cs;

namespace Cherris;

public class Camera : Node2D
{
    public float Zoom { get; set; } = 1;

    public void SetAsActive()
    {
        RenderManager.Instance.SetCamera(this);
    }

    public static implicit operator Camera2D(Camera camera)
    {
        return new()
        {
            Target = camera.GlobalPosition,
            Offset = WindowManager.Size / 2,
            Zoom = camera.Zoom
        };
    }
}