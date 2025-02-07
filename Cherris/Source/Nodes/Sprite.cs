using Cherris.DrawCommands;
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

    protected override void Draw()
    {
        base.Draw();

        if (Texture is null) return;
        
        TextureScaledDC textureScaledDC = new()
        {
            Texture = Texture,
            Position = GlobalPosition,
            Origin = Origin,
            Rotation = Rotation,
            Scale = Scale,
            FlipH = FlipH,
            FlipV = FlipV,
            Layer = Layer,
            Shader = Shader,
            UseShader = UseShader,
            UpdateShaderUniforms = UpdateShaderUniforms
        };

        RenderServer.Instance.Submit(textureScaledDC);
    }

    protected virtual void UpdateShaderUniforms(Shader shader) { }
}