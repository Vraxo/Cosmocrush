using Cherris;

namespace Cosmocrush;

public class CustomButton : Button
{
    private Tween? activeTween;

    public override void Ready()
    {
        base.Ready();

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseEntered(Button sender)
    {
        activeTween?.Stop();

        activeTween = Tree.CreateTween(this, ProcessMode.Always);
        activeTween.TweenProperty(this, "Themes/Hover/FontSize", 32, 0.25f);
        activeTween.TweenProperty(this, "Themes/Normal/FontSize", 32, 0.25f);
    }

    private void OnMouseExited(Button sender)
    {
        activeTween?.Stop();

        activeTween = Tree.CreateTween(this, ProcessMode.Always);
        activeTween.TweenProperty(this, "Themes/Hover/FontSize", 16, 0.25f);
        activeTween.TweenProperty(this, "Themes/Normal/FontSize", 16, 0.25f);
    }
}