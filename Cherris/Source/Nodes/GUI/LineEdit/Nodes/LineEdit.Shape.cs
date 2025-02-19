namespace Cherris;

public partial class LineEdit : Button
{
    private class Shape(LineEdit parent) : VisualItem
    {
        public void Update()
        {
            Draw();
        }

        public override void Draw()
        {
            if (!(parent.Visible && parent.ReadyForVisibility))
            {
                return;
            }

            DrawRectangleThemed(
                parent.GlobalPosition - parent.Offset,
                parent.Size,
                parent.Themes.Current);
        }
    }
}