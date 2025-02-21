namespace Cherris;

public class Shader
{
    private Raylib_cs.Shader raylibShader;

    private Shader(Raylib_cs.Shader shader)
    {
        raylibShader = shader;
    }

    public static Shader? Load(string? vertexShaderPath, string? fragmentShaderPath)
    {
        Raylib_cs.Shader shader = Raylib_cs.Raylib.LoadShader(vertexShaderPath, fragmentShaderPath);

        if (!Raylib_cs.Raylib.IsShaderReady(shader))
        {
            Log.Error($"Failed to load shader: {vertexShaderPath},{fragmentShaderPath}");
            return null;
        }

        return new(shader);
    }

    public static implicit operator Raylib_cs.Shader(Shader shader)
    {
        return shader.raylibShader;
    }
}