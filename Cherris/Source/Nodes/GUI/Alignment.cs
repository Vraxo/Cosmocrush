namespace Cherris;

public class Alignment
{
    public HAlignment Horizontal { get; set; } = HAlignment.Center;
    public VAlignment Vertical { get; set; } = VAlignment.Center;

    public Alignment() { }

    public Alignment(HAlignment horizontal, VAlignment vertical)
    {
        Horizontal = horizontal;
        Vertical = vertical;
    }

    public override string ToString()
    {
        return $"<{Horizontal}, {Vertical}?";
    }

    public static implicit operator string(Alignment alignment)
    {
        return alignment.ToString();
    }
}