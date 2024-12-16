namespace Nodica;

public sealed class FontManager
{
    private static FontManager? _instance;
    public static FontManager Instance => _instance ??= new();

    private readonly Dictionary<string, Font> fonts = [];

    private FontManager() { }

    public Font Get(string fullName)
    {
        if (fonts.TryGetValue(fullName, out Font? value))
        {
            return value;
        }

        (string fontName, int fontSize) = ParseFontName(fullName);

        string fontPath = $"Res/Nodica/Fonts/{fontName}.ttf";

        Font textFont = new(fontPath, fontSize);

        fonts.Add(fullName, textFont);

        return textFont;
    }

    private static (string fontName, int fontSize) ParseFontName(string fullName)
    {
        int lastSpaceIndex = fullName.LastIndexOf(' ');

        if (lastSpaceIndex == -1)
        {
            throw new ArgumentException($"Invalid font name format: {fullName}. Expected format: 'FontName Size'.");
        }

        string fontName = fullName[..lastSpaceIndex];
        string sizeString = fullName[(lastSpaceIndex + 1)..];

        if (!int.TryParse(sizeString, out int fontSize))
        {
            throw new ArgumentException($"Invalid font size in: {fullName}. Size must be a number.");
        }

        return (fontName, fontSize);
    }
}