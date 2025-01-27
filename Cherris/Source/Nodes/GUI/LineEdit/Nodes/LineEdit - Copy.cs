/*using Raylib_cs;

namespace Cherris;

public partial class LineEdit : Button
{
    #region [ - - - Properties & Fields - - - ]

    public static readonly Vector2 DefaultSize = new(300, 26);

    public new string TextDC { get; set; } = "";
    public string DefaultText { get; set; } = "";
    public string PlaceholderText { get; set; } = "";
    public Vector2 TextOrigin { get; set; } = new(8, 0);
    public int MaxCharacters { get; set; } = int.MaxValue;
    public int MinCharacters { get; set; } = 0;
    public List<char> ValidCharacters { get; set; } = [];
    public bool Selected { get; set; } = false;
    public bool Editable { get; set; } = true;
    public bool ExpandWidthToText { get; set; } = false;
    public bool RevertToDefaultText { get; set; } = true;
    public bool TemporaryDefaultText { get; set; } = true;
    public bool Secret { get; set; } = false;
    public char SecretCharacter { get; set; } = '*';
    public int TextStartIndex { get; private set; } = 0;

    public event EventHandler? FirstCharacterEntered;
    public event EventHandler? Cleared;
    public event EventHandler<string>? TextChanged;
    public event EventHandler<string>? Confirmed;

    protected Caret caret;
    private readonly TextDisplayer textDisplayer;
    private readonly PlaceholderTextDisplayer placeholderTextDisplayer;

    private const int minAscii = 32;
    private const int maxAscii = 125;
    private const float backspaceDelay = 0.5f;
    private const float backspaceSpeed = 0.05f;

    private float backspaceTimer = 0f;
    private bool backspaceHeld = false;

    private float previousWidth = 0;

    #endregion

    public LineEdit()
    {
        Size = DefaultSize;
        Focusable = true;
        Themes.Focused.BorderLength = 0;
        Themes.Focused.BorderLengthBottom = 1;

        caret = new(this);
        textDisplayer = new(this);
        placeholderTextDisplayer = new(this);

        FocusChanged += OnFocusChanged;
        LeftClicked += OnLeftClicked;
        ClickedOutside += OnClickedOutside;
        LayerChanged += OnLayerChanged;
        SizeChanged += OnSizeChanged;
    }

    public override void Update()
    {
        HandleInput();
        PasteText();
        UpdateSizeToFitText();

        base.Update();

        caret?.Update();
        textDisplayer?.Update();
        placeholderTextDisplayer?.Update();
    }

    protected override void OnEnterPressed()
    {
        Selected = false;
        Focused = false;
        Themes.Current = base.Themes.Normal;

        base.OnEnterPressed();
    }

    private void OnLayerChanged(VisualItem sender, int layer)
    {
        caret.Layer = layer + 1;
        textDisplayer.Layer = layer + 1;
        placeholderTextDisplayer.Layer = layer + 1;
    }

    private void OnClickedOutside(object? sender, EventArgs e)
    {
        Selected = false;
    }

    private void OnLeftClicked(Button sender)
    {
        Selected = true;
    }

    private void OnFocusChanged(object? sender, bool e)
    {
        Selected = e;
    }

    private void OnSizeChanged(object? sender, Vector2 e)
    {
        if (previousWidth != e.X)
        {
            previousWidth = e.X;
            TextStartIndex = 0;
        }
    }

    private void UpdateSizeToFitText()
    {
        if (!ExpandWidthToText)
        {
            return;
        }

        int textWidth = (int)Raylib.MeasureTextEx(
            Themes.Current.Font,
            TextDC,
            Themes.Current.FontSize,
            Themes.Current.FontSpacing).X;

        Size = new(textWidth + TextOrigin.X * 2, Size.Y);
    }

    public void Insert(string input)
    {
        if (!Editable)
        {
            return;
        }

        InsertTextAtCaret(input);
    }

    private void HandleInput()
    {
        if (!Editable)
        {
            return;
        }

        if (!Selected)
        {
            return;
        }

        GetTypedCharacters();
        HandleBackspace();
        Confirm();
    }

    private void GetTypedCharacters()
    {
        int key = Raylib.GetCharPressed();

        while (key > 0)
        {
            InsertCharacter(key);
            key = Raylib.GetCharPressed();
        }
    }

    private void InsertCharacter(int key)
    {
        bool isKeyInRange = key >= minAscii && key <= maxAscii;
        bool isSpaceLeft = TextDC.Length < MaxCharacters;

        if (isKeyInRange && isSpaceLeft)
        {
            if (ValidCharacters.Count > 0 && !ValidCharacters.Contains((char)key))
            {
                return;
            }

            if (TemporaryDefaultText && TextDC == DefaultText)
            {
                TextDC = "";
            }

            if (caret.X < 0 || caret.X > TextDC.Length)
            {
                caret.X = TextDC.Length;
            }

            TextDC = TextDC.Insert(caret.X + TextStartIndex, ((char)key).ToString());

            // Check if caret is out of view, and adjust TextStartIndex
            if (caret.X >= GetDisplayableCharactersCount())
            {
                TextStartIndex++;
            }
            else
            {
                caret.X++;
            }

            TextChanged?.Invoke(this, TextDC);

            if (TextDC.Length == 1)
            {
                FirstCharacterEntered?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void InsertTextAtCaret(string text)
    {
        bool isSpaceLeft = TextDC.Length + text.Length <= MaxCharacters;

        if (isSpaceLeft)
        {
            if (TemporaryDefaultText && TextDC == DefaultText)
            {
                TextDC = "";
            }

            if (caret.X < 0 || caret.X > TextDC.Length)
            {
                caret.X = TextDC.Length;
            }

            TextDC = TextDC.Insert(caret.X + TextStartIndex, text);
            caret.X += text.Length;

            // Shift text if caret moves out of view
            if (caret.X > GetDisplayableCharactersCount())
            {
                TextStartIndex = caret.X - GetDisplayableCharactersCount();
            }

            TextChanged?.Invoke(this, TextDC);

            if (TextDC.Length == text.Length)
            {
                FirstCharacterEntered?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void HandleBackspace()
    {
        bool ctrlHeld = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);
        bool backspacePressed = Raylib.IsKeyPressed(KeyboardKey.Backspace);

        if (backspacePressed)
        {
            backspaceHeld = true;
            backspaceTimer = 0f;

            if (ctrlHeld)
            {
                DeletePreviousWord();
            }
            else
            {
                DeleteLastCharacter();
            }
        }
        else if (Raylib.IsKeyDown(KeyboardKey.Backspace) && backspaceHeld)
        {
            backspaceTimer += Raylib.GetFrameTime();

            if (backspaceTimer >= backspaceDelay)
            {
                if (backspaceTimer % backspaceSpeed < Raylib.GetFrameTime())
                {
                    if (ctrlHeld)
                    {
                        DeletePreviousWord();
                    }
                    else
                    {
                        DeleteLastCharacter();
                    }
                }
            }
        }

        if (Raylib.IsKeyReleased(KeyboardKey.Backspace))
        {
            backspaceHeld = false;
        }
    }

    private void DeletePreviousWord()
    {
        if (TextDC.Length == 0 || (caret.X == 0 && TextStartIndex == 0)) return;

        // Calculate the actual index within the full text based on caret position and TextStartIndex
        int removeIndex = caret.X + TextStartIndex - 1;

        // If we're at the very start of the text that's displayed but there is more hidden to the left
        if (caret.X == 0 && TextStartIndex > 0)
        {
            // Shift TextStartIndex left, and move removeIndex accordingly
            TextStartIndex--;
            removeIndex = TextStartIndex;
        }

        // Find the start of the previous word by moving back from removeIndex
        int wordStartIndex = removeIndex;
        while (wordStartIndex > 0 && TextDC[wordStartIndex - 1] != ' ')
        {
            wordStartIndex--;
        }

        // Calculate the number of characters to delete
        int lengthToDelete = removeIndex - wordStartIndex + 1;
        TextDC = TextDC.Remove(wordStartIndex, lengthToDelete);

        // Adjust TextStartIndex if characters were deleted from the hidden portion
        if (wordStartIndex < TextStartIndex)
        {
            int charactersRemoved = removeIndex - wordStartIndex + 1;
            TextStartIndex = Math.Max(0, TextStartIndex - charactersRemoved);
        }

        // Reposition the caret
        caret.X = Math.Clamp(wordStartIndex - TextStartIndex, 0, GetDisplayableCharactersCount());

        RevertTextToDefaultIfEmpty();
        TextChanged?.Invoke(this, TextDC);

        // Trigger Cleared event if the text is now empty
        if (TextDC.Length == 0)
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DeleteLastCharacter()
    {
        int textLengthBeforeDeletion = TextDC.Length;

        if (TextDC.Length > 0)
        {
            if (caret.X == 0 && TextStartIndex > 0)
            {
                TextStartIndex--;
                TextDC = TextDC.Remove(TextStartIndex, 1);
            }
            else if (caret.X > 0)
            {
                int removeIndex = caret.X - 1 + TextStartIndex;
                if (removeIndex >= TextStartIndex && removeIndex < TextDC.Length)
                {
                    TextDC = TextDC.Remove(removeIndex, 1);
                    caret.X = Math.Clamp(caret.X - 1, 0, Math.Min(TextDC.Length, GetDisplayableCharactersCount()));
                }
            }
        }

        RevertTextToDefaultIfEmpty();
        TextChanged?.Invoke(this, TextDC);

        if (TextDC.Length == 0 && textLengthBeforeDeletion != 0)
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PasteText()
    {
        bool pressedLeftControl = Raylib.IsKeyDown(KeyboardKey.LeftControl);
        bool pressedV = Raylib.IsKeyPressed(KeyboardKey.V);

        if (pressedLeftControl && pressedV)
        {
            string clipboardText = Raylib.GetClipboardText_();

            if (!string.IsNullOrEmpty(clipboardText))
            {
                char[] clipboardContent = [..clipboardText];

                foreach (char character in clipboardContent)
                {
                    InsertCharacter(character);
                }
            }
        }
    }

    private void Confirm()
    {
        if (Raylib.IsKeyDown(KeyboardKey.Enter))
        {
            Selected = false;
            Themes.Current = Themes.Normal;
            Confirmed?.Invoke(this, TextDC);
        }
    }

    private void RevertTextToDefaultIfEmpty()
    {
        if (TextDC.Length == 0)
        {
            TextDC = DefaultText;
        }
    }

    private int GetDisplayableCharactersCount()
    {
        float availableWidth = Size.X - TextOrigin.X * 2;

        float oneCharacterWidth = Raylib.MeasureTextEx(
            Themes.Current.Font,
            ".",
            Themes.Current.FontSize,
            Themes.Current.FontSpacing).X;

        int displayableCharactersCount = (int)(availableWidth / oneCharacterWidth);

        return displayableCharactersCount;
    }
}
*/