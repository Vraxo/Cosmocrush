using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cherris;

public static class PackedSceneUtils
{
    private static readonly string[] SpecialProperties = { "type", "name", "path" };

    public static void SetProperties(Node node, Dictionary<string, object> element)
    {
        foreach (KeyValuePair<string, object> property in element)
        {
            string propertyName = property.Key;
            if (SpecialProperties.Contains(propertyName))
            {
                continue;
            }

            object value = property.Value;
            SetNestedProperty(node, propertyName, value);
        }
    }

    private static void SetNestedProperty(object target, string propertyPath, object value)
    {
        string[] pathParts = propertyPath.Split('/');
        object currentObject = target;

        for (int i = 0; i < pathParts.Length; i++)
        {
            string part = pathParts[i];
            PropertyInfo? propertyInfo = currentObject.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new Exception($"Property '{part}' not found on type '{currentObject.GetType().Name}'.");
            }

            if (i == pathParts.Length - 1)
            {
                // Final part of the path, set the value
                object propertyValue = ConvertValue(propertyInfo.PropertyType, value);
                propertyInfo.SetValue(currentObject, propertyValue);
            }
            else
            {
                // Intermediate part of the path, navigate or create nested object
                object? nextObject = propertyInfo.GetValue(currentObject);
                if (nextObject == null)
                {
                    nextObject = Activator.CreateInstance(propertyInfo.PropertyType);
                    propertyInfo.SetValue(currentObject, nextObject);
                }
                currentObject = nextObject!;
            }
        }
    }

    public static Type ResolveType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? type = assembly.GetType(typeName, false, true);
            if (type != null)
                return type;

            string defaultNamespace = assembly.GetName().Name!;
            string namespacedTypeName = defaultNamespace + "." + typeName;
            type = assembly.GetType(namespacedTypeName, false, true);
            if (type != null)
                return type;
        }

        throw new Exception($"Type '{typeName}' not found.");
    }

    public static object ConvertValue(Type targetType, object value)
    {
        if (value is Dictionary<object, object> dict)
        {
            return ConvertNestedObject(targetType, dict);
        }

        var stringValue = value.ToString() ?? throw new Exception("Value cannot be null");

        // Remove outer quotes if present
        if ((stringValue.StartsWith("\"") && stringValue.EndsWith("\"")) ||
            (stringValue.StartsWith("'") && stringValue.EndsWith("'")))
        {
            stringValue = stringValue.Substring(1, stringValue.Length - 2);
        }

        return targetType switch
        {
            Type t when t == typeof(Vector2) => ParseVector2(stringValue),
            Type t when t == typeof(Color) => ParseColor(stringValue),
            Type t when t == typeof(Audio) => ParseAudio(stringValue),
            Type t when t == typeof(Texture) => ParseTexture(stringValue),
            Type t when t == typeof(Font) => ResourceLoader.Load<Font>(stringValue),
            Type t when t.IsEnum => Enum.Parse(targetType, stringValue),
            Type t when t == typeof(int) => int.Parse(stringValue),
            Type t when t == typeof(uint) => uint.Parse(stringValue),
            Type t when t == typeof(float) => float.Parse(stringValue),
            Type t when t == typeof(double) => double.Parse(stringValue),
            Type t when t == typeof(bool) => bool.Parse(stringValue),
            Type t when t == typeof(string) => stringValue,
            _ => throw new Exception($"Unsupported type {targetType.Name} for value {stringValue}")
        };
    }

    private static object ConvertNestedObject(Type targetType, Dictionary<object, object> dict)
    {
        var targetObject = Activator.CreateInstance(targetType) ?? throw new Exception($"Failed to create instance of type {targetType.Name}");

        foreach (var kvp in dict)
        {
            string key = kvp.Key.ToString()!;
            object value = kvp.Value;

            var propertyInfo = targetType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new Exception($"Property '{key}' not found on type '{targetType.Name}'.");
            }

            object convertedValue = ConvertValue(propertyInfo.PropertyType, value);
            propertyInfo.SetValue(targetObject, convertedValue);
        }

        return targetObject;
    }

    private static Vector2 ParseVector2(string value)
    {
        string[] parts = value.Trim('(', ')').Split(',');

        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);

        return new(x, y);
    }

    private static Color ParseColor(string value)
    {
        string[] parts = value.Trim('(', ')').Split(',');

        byte r = byte.Parse(parts[0]);
        byte g = byte.Parse(parts[1]);
        byte b = byte.Parse(parts[2]);
        byte a = parts.Length > 3 ? byte.Parse(parts[3]) : (byte)255;

        return new(r, g, b, a);
    }

    private static Audio ParseAudio(string path)
    {
        return new(path);
    }

    private static Texture ParseTexture(string path)
    {
        return TextureManager.Instance.Get(path);
    }
}
