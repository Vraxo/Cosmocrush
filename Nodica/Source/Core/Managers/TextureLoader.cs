﻿using Raylib_cs;

namespace Nodica;

public class TextureLoader
{
    public static TextureLoader Instance => instance ??= new();
    private static TextureLoader? instance;

    private Dictionary<string, Texture> textures = new();

    private TextureLoader() { }

    public Texture Get(string path)
    {
        if (textures.ContainsKey(path))
            return textures[path];

        Texture textureWrapper = new(path);
        textures[path] = textureWrapper;

        return textureWrapper;
    }

    public void Remove(string path)
    {
        if (textures.ContainsKey(path))
        {
            Raylib.UnloadTexture(textures[path]);
            textures.Remove(path);

            string pngPath = Path.GetExtension(path).ToLower() == ".png"
                ? path
                : $"Res/Temporary/{Path.GetFileNameWithoutExtension(path)}.png";

            if (pngPath != path && File.Exists(pngPath))
                File.Delete(pngPath);
        }
    }
}