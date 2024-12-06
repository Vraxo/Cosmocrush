using Raylib_cs;
using System.Text.Json;

namespace Nodica;

public sealed class Input
{
    private static readonly Input _instance = new Input();
    public static Input Instance => _instance;

    private readonly Dictionary<string, List<InputAction>> actionKeyMap = new();
    private readonly string mappingsFilePath = "Resources/Nodica/InputMappings.json";

    public static Vector2 MousePosition => Instance._GetMousePosition();
    public static bool IsActionDown(string actionName) => Instance._IsActionDown(actionName);
    public static bool IsActionPressed(string actionName) => Instance._IsActionPressed(actionName);
    public static bool IsKeyPressed(Key keyboardKey) => Instance._IsKeyPressed(keyboardKey);
    public static bool IsKeyReleased(Key keyboardKey) => Instance._IsKeyReleased(keyboardKey);
    public static bool IsKeyDown(Key keyboardKey) => Instance._IsKeyDown(keyboardKey);
    public static bool IsMouseButtonPressed(MouseKey button) => Instance._IsMouseButtonPressed(button);
    public static bool IsMouseButtonReleased(MouseKey button) => Instance._IsMouseButtonReleased(button);
    public static bool IsMouseButtonDown(MouseKey button) => Instance._IsMouseButtonDown(button);
    public static Vector2 GetVector(string negative_x, string positive_x, string negative_y, string positive_y, float deadzone = -1.0f)
        => Instance._GetVector(negative_x, positive_x, negative_y, positive_y, deadzone);

    private Input()
    {
        _LoadMappings(mappingsFilePath);
    }

    private bool _IsActionDown(string actionName)
    {
        if (actionKeyMap.TryGetValue(actionName, out List<InputAction> actions))
        {
            foreach (var action in actions)
            {
                if (action.IsDown())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool _IsActionPressed(string actionName)
    {
        if (actionKeyMap.TryGetValue(actionName, out List<InputAction> actions))
        {
            foreach (var action in actions)
            {
                if (action.IsPressed())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool _IsKeyPressed(Key keyboardKey)
    {
        return Raylib.IsKeyPressed((Raylib_cs.KeyboardKey)keyboardKey);
    }

    private bool _IsKeyReleased(Key keyboardKey)
    {
        return Raylib.IsKeyReleased((Raylib_cs.KeyboardKey)keyboardKey);
    }

    private bool _IsKeyDown(Key keyboardKey)
    {
        return Raylib.IsKeyDown((Raylib_cs.KeyboardKey)keyboardKey);
    }

    private bool _IsMouseButtonPressed(MouseKey button)
    {
        return Raylib.IsMouseButtonPressed((Raylib_cs.MouseButton)button);
    }

    private bool _IsMouseButtonReleased(MouseKey button)
    {
        return Raylib.IsMouseButtonReleased((Raylib_cs.MouseButton)button);
    }

    private bool _IsMouseButtonDown(MouseKey button)
    {
        return Raylib.IsMouseButtonDown((Raylib_cs.MouseButton)button);
    }

    private Vector2 _GetMousePosition()
    {
        return Raylib.GetMousePosition();
    }

    private Vector2 _GetVector(string negative_x, string positive_x, string negative_y, string positive_y, float deadzone = -1.0f)
    {
        float x = 0.0f;
        float y = 0.0f;

        if (_IsActionDown(negative_x)) x -= 1.0f;
        if (_IsActionDown(positive_x)) x += 1.0f;
        if (_IsActionDown(negative_y)) y -= 1.0f;
        if (_IsActionDown(positive_y)) y += 1.0f;

        if (deadzone >= 0.0f)
        {
            if (MathF.Abs(x) < deadzone) x = 0.0f;
            if (MathF.Abs(y) < deadzone) y = 0.0f;
        }

        return new Vector2(x, y);
    }

    private void _LoadMappings(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var mappings = JsonSerializer.Deserialize<Dictionary<string, List<InputAction>>>(json);
        actionKeyMap.Clear();

        foreach (var mapping in mappings)
        {
            actionKeyMap[mapping.Key] = mapping.Value;
        }
    }
}