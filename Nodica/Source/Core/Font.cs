using Raylib_cs;

namespace Nodica;

public class Font
{
    public int Size = 0;

    public Vector2 Dimensions
    {
        get
        {
            Vector2 dimensions = Raylib.MeasureTextEx(
                this,
                " ",
                Size,
                0);

            return dimensions;
        }
    }

    private Raylib_cs.Font font;

    public Font(string filePath, int size)
    {
        int[] codepoints = new int[255 - 32 + 1];
        for (int i = 0; i < codepoints.Length; i++)
            codepoints[i] = 32 + i;

        Size = size;

        font = Raylib.LoadFontEx(filePath, size, codepoints, codepoints.Length);
        Raylib.SetTextureFilter(font.Texture, TextureFilter.Bilinear);
    }

    public static implicit operator Raylib_cs.Font(Font textFont) => textFont.font;
}