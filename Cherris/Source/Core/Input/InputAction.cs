namespace Cherris;

public sealed class InputAction
{
    public string? Type { get; set; }
    public string? KeyOrButton { get; set; }

    public bool IsPressed()
    {
        if (Type == "Keyboard")
        {
            if (Enum.TryParse(KeyOrButton, true, out KeyCode key))
            {
                return Input.IsKeyPressed(key);
            }
        }
        else if (Type == "MouseButton")
        {
            if (Enum.TryParse(KeyOrButton, true, out MouseButtonCode button))
            {
                return Input.IsMouseButtonPressed(button);
            }
        }

        return false;
    }

    public bool IsDown()
    {
        if (Type == "Keyboard")
        {
            if (Enum.TryParse(KeyOrButton, true, out KeyCode key))
            {
                return Input.IsKeyDown(key);
            }
        }
        else if (Type == "MouseButton")
        {
            if (Enum.TryParse(KeyOrButton, true, out MouseButtonCode button))
            {
                return Input.IsMouseButtonDown(button);
            }
        }

        return false;
    }
}
