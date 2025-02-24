using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class InputServer
{
    public static InputServer Instance { get; } = new();

    private readonly Dictionary<string, List<InputAction>> actionKeyMap = [];
    private readonly string mappingsFilePath = "Res/Cherris/InputMap.yaml";

    private InputServer()
    {
        LoadMappings(mappingsFilePath);
    }

    // Actions

    internal bool IsActionPressed(string actionName)
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

    internal bool IsActionJustPressed(string actionName)
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

    // Keyboard

    internal static bool IsKeyPressed(KeyCode keyboardKey)
    {
        return Raylib.IsKeyPressed((KeyboardKey)keyboardKey);
    }

    internal static bool IsKeyReleased(KeyCode keyboardKey)
    {
        return Raylib.IsKeyReleased((KeyboardKey)keyboardKey);
    }

    internal static bool IsKeyDown(KeyCode keyboardKey)
    {
        return Raylib.IsKeyDown((KeyboardKey)keyboardKey);
    }

    // Mouse

    internal static bool IsMouseButtonPressed(MouseButtonCode button)
    {
        return Raylib.IsMouseButtonPressed((MouseButton)button);
    }

    internal static bool IsMouseButtonReleased(MouseButtonCode button)
    {
        return Raylib.IsMouseButtonReleased((MouseButton)button);
    }

    internal static bool IsMouseButtonDown(MouseButtonCode button)
    {
        return Raylib.IsMouseButtonDown((MouseButton)button);
    }

    internal static float MouseWheelMovement
    {
        get
        {
            return Raylib.GetMouseWheelMove();
        }
    }

    internal static Vector2 MousePosition
    {
        get
        {
            return Raylib.GetMousePosition();
        }
    }

    internal static Vector2 WorldMousePosition
    {
        get => RenderServer.Instance.GetScreenToWorld(MousePosition);
    }

    internal static MouseCursorCode Cursor
    {
        set => Raylib.SetMouseCursor((MouseCursor)value);
    }

    // Other

    internal Vector2 GetVector(string negativeX, string positiveX, string negativeY, string positiveY, float deadzone = -1.0f)
    {
        float x = 0.0f;
        float y = 0.0f;

        if (IsActionPressed(negativeX))
        {
            x -= 1.0f;
        }

        if (IsActionPressed(positiveX))
        {
            x += 1.0f;
        }

        if (IsActionPressed(negativeY))
        {
            y -= 1.0f;
        }

        if (IsActionPressed(positiveY))
        {
            y += 1.0f;
        }

        if (deadzone >= 0.0f)
        {
            if (float.Abs(x) < deadzone)
            {
                x = 0.0f;
            }

            if (float.Abs(y) < deadzone)
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
