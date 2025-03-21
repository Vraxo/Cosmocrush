﻿namespace Cherris;

public partial class OptionButton : Button
{
    public class OptionButtonButton : Button
    {
        public int Index = 0;
        public bool Selected = false;
        public BoxStyle CheckTheme = new();

        private OptionButton parent;

        public OptionButtonButton()
        {
            CheckTheme.Roundness = 1;
            CheckTheme.FillColor = DefaultTheme.Accent;
        }

        public override void Start()
        {
            TextAlignment.Horizontal = HAlignment.Right;
            TextOffset = new(-4, 0);

            parent = GetParent<OptionButton>();

            LeftClicked += OnLeftClicked;

            base.Start();
        }

        public override void Draw()
        {
            base.Draw();

            if (!Selected)
            {
                return;
            }

            DrawRectangleThemed(
                GlobalPosition - Origin + new Vector2(10, 7.5f),
                new(10, 10),
                CheckTheme);
        }

        private void OnLeftClicked(Button sender)
        {
            parent.Select(Index);

            if (Focused)
            {
                parent.Focused = true;
            }
        }
    }
}