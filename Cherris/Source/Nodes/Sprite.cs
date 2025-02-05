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
    public float FlashValue { get; set; } = 1f;

    public Sprite()
    {
        UseShader = true;
        Shader = Raylib.LoadShader(null, "Res/Shaders/HitFlash.shader");
        flashColorLoc = Raylib.GetShaderLocation(Shader, "flash_color");
        flashValueLoc = Raylib.GetShaderLocation(Shader, "flash_value");
    }

    protected override void Draw()
    {
        base.Draw();

        if (Texture is null) return;

        // Convert flash color to [0.0, 1.0] range for shader
        float[] flashColorData = new float[]
        {
            FlashColor.R / 255.0f,
            FlashColor.G / 255.0f,
            FlashColor.B / 255.0f,
            FlashColor.A / 255.0f
        };

        // Prepare the TextureScaledDC and submit it
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
                // Pass the shader uniform values
                Raylib.SetShaderValue(shader, flashValueLoc, new[] { FlashValue }, ShaderUniformDataType.Float);
                Raylib.SetShaderValue(shader, flashColorLoc, flashColorData, ShaderUniformDataType.Vec4);
                // No need to manually bind the texture; Raylib handles this automatically
            }
        };

        // Submit the draw command to the renderer
        RenderServer.Instance.Submit(textureScaledDC);
    }
}