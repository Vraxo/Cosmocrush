using Cherris;

namespace Cosmocrush;

public class EnemySprite : Sprite
{
    private int flashColorLoc;
    private int flashValueLoc;

    private Color flashColor = Color.White;
    public float flashValue = 0f;
    
    public override void Ready()
    {
        base.Ready();

        Shader = Cherris.Shader.Load(null, "Res/Shaders/HitFlash.shader");

        flashColorLoc = GetShaderLocation("flash_color");
        flashValueLoc = GetShaderLocation("flash_value");
    }

    protected override void UpdateShaderUniforms(Cherris.Shader shader)
    {
        float[] flashColorData =
        [
            flashColor.R / 255.0f,
            flashColor.G / 255.0f,
            flashColor.B / 255.0f,
            flashColor.A / 255.0f
        ];

        SetShaderValue(flashValueLoc, [flashValue], ShaderUniformDataType.Float);
        SetShaderValue(flashColorLoc, flashColorData, ShaderUniformDataType.Vec4);
    }
}