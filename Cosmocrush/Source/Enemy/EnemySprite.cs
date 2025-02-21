﻿using Cherris;
using Raylib_cs;

namespace Cosmocrush;

public class EnemySprite : Sprite
{
    private int flashColorLoc;
    private int flashValueLoc;

    public Color flashColor { get; set; } = Color.White;
    public float flashValue { get; set; } = 0f;

    public override void Ready()
    {
        base.Ready();

        Shader = Cherris.Shader.Load(null, "Res/Shaders/HitFlash.shader");

        flashColorLoc = Raylib.GetShaderLocation(Shader, "flash_color");
        flashValueLoc = Raylib.GetShaderLocation(Shader, "flash_value");
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

        Raylib.SetShaderValue(shader, flashValueLoc, new[] { flashValue }, ShaderUniformDataType.Float);
        Raylib.SetShaderValue(shader, flashColorLoc, flashColorData, ShaderUniformDataType.Vec4);
    }
}