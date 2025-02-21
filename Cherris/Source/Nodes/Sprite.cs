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
            if (Shader is not null)
            {
                UpdateShaderUniforms(Shader);
                RenderServer.BeginShaderMode(Shader);
            }

            Raylib.DrawTexturePro(
                Texture,
                new()
                {
                    Position = new(0, 0),
                    Width = Texture.Size.X * (FlipH ? -1 : 1),
                    Height = Texture.Size.Y * (FlipV ? -1 : 1),
                },
                new()
                {
                    Position = GlobalPosition,
                    Size = Texture.Size * Scale
                },
                Origin,
                Rotation,
                Color.White);

            if (Shader is not null)
            {
                RenderServer.EndShaderMode();
            }

        }, Layer);
    }

    protected virtual void UpdateShaderUniforms(Shader shader)
    {
        if (Texture is null)
        {
            return;
        }

        int textureLoc = Raylib.GetShaderLocation(shader, "texture0");
        Raylib.SetShaderValueTexture(shader, textureLoc, Texture);
    }
}