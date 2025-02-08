namespace Cherris;

public static class Input
{
    // Actions

    public static bool IsActionDown(string actionName)
    {
        return InputServer.Instance.IsActionPressed(actionName);
    }

    public static bool IsActionPressed(string actionName)
    {
        return InputServer.Instance.IsActionJustPressed(actionName);
    }

    // keyboard

    public static bool IsKeyPressed(KeyCode keyboardKey)
    {
        return InputServer.IsKeyPressed(keyboardKey);
    }

    public static bool IsKeyReleased(KeyCode keyboardKey)
    {
        return InputServer.IsKeyReleased(keyboardKey);
    }

    public static bool IsKeyDown(KeyCode keyboardKey)
    {
        return InputServer.IsKeyDown(keyboardKey);
    }

    // Mouse

    public static bool IsMouseButtonPressed(MouseButtonCode button)
    {
        return InputServer.IsMouseButtonPressed(button);
    }

    public static bool IsMouseButtonReleased(MouseButtonCode button)
    {
        return InputServer.IsMouseButtonReleased(button);
    }

    public static bool IsMouseButtonDown(MouseButtonCode button)
    {
        return InputServer.IsMouseButtonDown(button);
    }

    public static float GetMouseWheelMovement()
    {
        return InputServer.MouseWheelMovement;
    }

    public static Vector2 MousePosition
    {
        get
        {
            return InputServer.MousePosition;
        }
    }

    public static Vector2 WorldMousePosition
    {
        get
        {
            return InputServer.WorldMousePosition;
        }
    }

    public static MouseCursorCode Cursor
    {
        set => InputServer.Cursor = value;
    }

    // Other

    public static Vector2 GetVector(string negativeX, string positiveX, string negativeY, string positiveY, float deadzone = -1.0f)
    {
        return InputServer.Instance.GetVector(negativeX, positiveX, negativeY, positiveY, deadzone);
    }
}