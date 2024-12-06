using Raylib_cs;

public class InputAction
{
    public string Type { get; set; }
    public string KeyOrButton { get; set; }

    public bool IsPressed()
    {
        if (Type == "Keyboard")
        {
            if (Enum.TryParse(KeyOrButton, true, out KeyboardKey key))
            {
                return Raylib.IsKeyPressed((Raylib_cs.KeyboardKey)key);
            }
        }
        else if (Type == "MouseButton")
        {
            if (Enum.TryParse(KeyOrButton, true, out MouseButton button))
            {
                return Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)button);
            }
        }
        return false;
    }

    public bool IsDown()
    {
        if (Type == "Keyboard")
        {
            if (Enum.TryParse(KeyOrButton, true, out KeyboardKey key))
            {
                return Raylib.IsKeyDown((Raylib_cs.KeyboardKey)key);
            }
        }
        else if (Type == "MouseButton")
        {
            if (Enum.TryParse(KeyOrButton, true, out MouseButton button))
            {
                return Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)button);
            }
        }
        return false;
    }
}