using Cherris.DrawCommands;
using Raylib_cs;

namespace Cherris;

public class Sprite : Node2D
{
    private int flashColorLoc;
    private int flashValueLoc;

    public bool FlipH { get; set; } = false;
    public bool FlipV { get; set; } = false;

    private Texture? _texture = null;
    public Texture? Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            Size = _texture?.Size ?? Size;
        }
    }

    public Color FlashColor { get; set; } = Color.White;
    public float FlashValue { get; set; } = 0f;

    public Sprite()
    {
        UseShader = true;
        Shader = Raylib.LoadShader(null, "Res/Shaders/HitFlash.fs");
        flashColorLoc = Raylib.GetShaderLocation(Shader, "flash_color");
        flashValueLoc = Raylib.GetShaderLocation(Shader, "flash_value");
    }

    protected override void Draw()
    {
        base.Draw();

        if (Texture is null)
        {
            return;
        }

        // Create and configure the draw command.
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
            UpdateShaderUniforms = (shader) =>
            {
                Raylib.SetShaderValue(shader, Raylib.GetShaderLocation(shader, "texture0"), ((Texture2D)Texture).Id, ShaderUniformDataType.Sampler2D);
                Raylib.SetShaderValue(shader, Raylib.GetShaderLocation(shader, "flash_value"), flashValueLoc, ShaderUniformDataType.Float);
                Raylib.SetShaderValue(shader, Raylib.GetShaderLocation(shader, "flash_color"), flashColorLoc, ShaderUniformDataType.Vec4);
            }
        };

        // Submit the draw command.
        RenderServer.Instance.Submit(textureScaledDC);
    }
}