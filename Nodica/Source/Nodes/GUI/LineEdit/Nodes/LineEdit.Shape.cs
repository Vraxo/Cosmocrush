namespace Nodica;

public partial class LineEdit : Button
{
    private class Shape : VisualItem
    {
        private LineEdit parent;

        public Shape(LineEdit parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            Draw();
        }

        private void Draw()
        {
            if (!(parent.Visible && parent.ReadyForVisibility))
            {
                return;
            }

            DrawThemedRectangle(
                parent.GlobalPosition - parent.Offset,
                parent.Size,
                parent.Themes.Current);
        }
    }
}