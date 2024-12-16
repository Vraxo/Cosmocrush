namespace Nodica;

public class Configuration
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int MinWidth { get; set; }
    public int MinHeight { get; set; }
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public string Title { get; set; }
    public bool ResizableWindow { get; set; }
    public bool AntiAliasing { get; set; }
    public string MainScenePath { get; set; }
    public string Backend { get; set; }
}