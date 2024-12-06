using Raylib_cs;
using SixLabors.ImageSharp;

namespace Nodica;

public class Texture
{
    public Vector2 Size { get; private set; } = Vector2.Zero;
    private Texture2D textureData;

    public Texture(string filePath)
    {
        string pngPath =
            Path.GetExtension(filePath).ToLower() == ".png" ?
            filePath :
            ConvertToPng(filePath);

        textureData = Raylib.LoadTexture(pngPath);

        if (textureData.Id == 0)
        {
            //Log.Warning("[Texture] Failed to load.", log);
        }
        else
        {
            //Log.Info("[Texture] Loaded texture.", log);
        }

        Size = new(textureData.Width, textureData.Height);

        if (pngPath != filePath)
        {
            File.Delete(pngPath);
        }
    }

    public Texture2D TextureData => textureData;

    public static implicit operator Texture2D(Texture texture) => texture.textureData;

    private string ConvertToPng(string imagePath)
    {
        if (!Directory.Exists("Res/Nodica/Temporary"))
        {
            Directory.CreateDirectory("Res/Temporary");
        }

        string pngPath = $"Res/Nodica/Temporary/{Path.GetFileNameWithoutExtension(imagePath)}.png";

        if (!File.Exists(pngPath))
        {
            using var image = SixLabors.ImageSharp.Image.Load(imagePath);
            image.SaveAsPng(pngPath);
        }

        //Log.Info("[Texture] Converted to PNG.", log);

        return pngPath;
    }
}