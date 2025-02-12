using Raylib_cs;

namespace Cherris;

public class Sprite : Node2D
{
    public bool FlipH { get; set; } = false;
    public bool FlipV { get; set; } = false;

    public Texture? Texture
    {
        get;

        set
        {
            field = value;
            Size = field?.Size ?? Size;
        }
    } = null;

    public override void Draw()
    {
        base.Draw();

        if (Texture is null)
        {
            return;
        }

        RenderServer.Instance.Submit(() =>
        {
            Rectangle source = new()
            {
                Position = new(0, 0),
                Width = Texture.Size.X * (FlipH ? -1 : 1),
                Height = Texture.Size.Y * (FlipV ? -1 : 1),
            };

            Rectangle destination = new()
            {
                Position = GlobalPosition,
                Size = Texture.Size * Scale
            };

            if (UseShader)
            {
                UpdateShaderUniforms(Shader);
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

        }, Layer);
    }

    protected virtual void UpdateShaderUniforms(Shader shader) { }
}