using Raylib_cs;

namespace Cherris.DrawCommands;

public class TextureScaledDC : DrawCommand
{
    public Texture Texture { get; set; } = new();
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public bool FlipH { get; set; } = false;
    public bool FlipV { get; set; } = false;

    public Action<Shader>? UpdateShaderUniforms { get; set; }

    public override void Draw()
    {
        Rectangle source = new()
        {
            Position = new(0, 0),
            Width = Texture.Size.X * (FlipH ? -1 : 1),
            Height = Texture.Size.Y * (FlipV ? -1 : 1),
        };

        Rectangle destination = new()
        {
            Position = Position,
            Size = Texture.Size * Scale
        };

        if (UseShader)
        {
            UpdateShaderUniforms?.Invoke(Shader);
            Raylib.BeginShaderMode(Shader);
        }

        Raylib.DrawTexturePro(
            Texture,
            source,
            destination,
            Origin,
            Rotation,
            Color.White);

        if (UseShader)
        {
            Raylib.EndShaderMode();
        }
    }
}