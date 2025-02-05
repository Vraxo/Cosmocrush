using Raylib_cs;

namespace Cherris;

public abstract class DrawCommand
{
    public int Layer { get; set; } = 0;
    public Shader Shader { get; set; }
    public bool UseShader { get; set; } = false;

    public abstract void Draw();
}