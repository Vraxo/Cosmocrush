using Raylib_cs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class Input
{
    private static Input? _instance;
    public static Input Instance => _instance ??= new();

    private readonly Dictionary<string, List<InputAction>> actionKeyMap = [];
    private readonly string mappingsFilePath = "Res/Cherris/InputMappings.yaml";

    public static MouseCursorCode Cursor
    {
        set
        {
            Instance.CursorImpl = value;
        }
    }

    public static Vector2 MousePosition => Instance.GetMousePositionImpl();

    public static Vector2 WorldMousePosition => Instance.GetWorldMousePositionImpl();

    public static float GetMouseWheelMovement() => Instance.GetMouseWheelMovementImpl();

    public static bool IsActionDown(string actionName)
    {
        return Instance.IsActionDownImpl(actionName);
    }

    public static bool IsActionPressed(string actionName)
    {
        return Instance.IsActionPressedImpl(actionName);
    }

    public static bool IsKeyPressed(KeyCode keyboardKey)
    {
        return Instance.IsKeyPressedImpl(keyboardKey);
    }

    public static bool IsKeyReleased(KeyCode keyboardKey)
    {
        return Instance.IsKeyReleasedImpl(keyboardKey);
    }

    public static bool IsKeyDown(KeyCode keyboardKey)
    {
        return Instance.IsKeyDownImpl(keyboardKey);
    }

    public static bool IsMouseButtonPressed(MouseButtonCode button)
    {
        return Instance.IsMouseButtonPressedImpl(button);
    }

    public static bool IsMouseButtonReleased(MouseButtonCode button)
    {
        return Instance.IsMouseButtonReleasedImpl(button);
    }

    public static bool IsMouseButtonDown(MouseButtonCode button)
    {
        return Instance.IsMouseButtonDownImpl(button);
    }

    public static Vector2 GetVector(string _x, string x, string _y, string y, float deadzone = -1.0f)
    {
        return Instance.GetVectorImpl(_x, x, _y, y, deadzone);
    }


    private Input()
    {
        LoadMappings(mappingsFilePath);
    }

    private MouseCursorCode CursorImpl
    {
        set
        {
            Raylib_cs.Raylib.SetMouseCursor((Raylib_cs.MouseCursor)value);
        }
    }

    private bool IsActionDownImpl(string actionName)
    {
        if (actionKeyMap.TryGetValue(actionName, out List<InputAction>? actions))
        {
            foreach (InputAction action in actions)
            {
                if (action.IsDown())
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsActionPressedImpl(string actionName)
    {
        if (actionKeyMap.TryGetValue(actionName, out List<InputAction>? actions))
        {
            foreach (InputAction action in actions)
            {
                if (action.IsPressed())
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsKeyPressedImpl(KeyCode keyboardKey)
    {
        return Raylib.IsKeyPressed((KeyboardKey)keyboardKey);
    }

    private bool IsKeyReleasedImpl(KeyCode keyboardKey)
    {
        return Raylib.IsKeyReleased((KeyboardKey)keyboardKey);
    }

    private bool IsKeyDownImpl(KeyCode keyboardKey)
    {
        return Raylib.IsKeyDown((KeyboardKey)keyboardKey);
    }

    private bool IsMouseButtonPressedImpl(MouseButtonCode button)
    {
        return Raylib.IsMouseButtonPressed((MouseButton)button);
    }

    private bool IsMouseButtonReleasedImpl(MouseButtonCode button) 
    {
        return Raylib.IsMouseButtonReleased((MouseButton)button);
    }

    private bool IsMouseButtonDownImpl(MouseButtonCode button) 
    {
        return Raylib.IsMouseButtonDown((MouseButton)button);
    }

    private float GetMouseWheelMovementImpl()
    {
        return Raylib.GetMouseWheelMove();
    }

    private Vector2 GetMousePositionImpl()
    {
        return Raylib.GetMousePosition();
    }

    private Vector2 GetWorldMousePositionImpl()
    {
        Camera2D camera = (Raylib_cs.Camera2D)RenderManager.Instance.Camera;
        return Raylib.GetScreenToWorld2D(GetMousePositionImpl(), camera);
    }

    private Vector2 GetVectorImpl(string negativeX, string positiveX, string negativeY, string positiveY, float deadzone = -1.0f)
    {
        float x = 0.0f;
        float y = 0.0f;

        if (IsActionDownImpl(negativeX))
        {
            x -= 1.0f;
        }

        if (IsActionDownImpl(positiveX))
        {
            x += 1.0f;
        }

        if (IsActionDownImpl(negativeY))
        {
            y -= 1.0f;
        }

        if (IsActionDownImpl(positiveY))
        {
            y += 1.0f;
        }

        if (deadzone >= 0.0f)
        {
            if (MathF.Abs(x) < deadzone)
            {
                x = 0.0f;
            }

            if (MathF.Abs(y) < deadzone)
            {
                y = 0.0f;
            }
        }

        return new(x, y);
    }

    private void LoadMappings(string filePath)
    {
        string yaml = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build();

        var mappings = deserializer.Deserialize<Dictionary<string, List<InputAction>>>(yaml);

        if (mappings is null)
        {
            return;
        }

        actionKeyMap.Clear();

        foreach (var mapping in mappings)
        {
            actionKeyMap[mapping.Key] = mapping.Value;
        }
    }
}