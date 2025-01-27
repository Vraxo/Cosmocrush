using System;
using System.Collections;
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

    public static void SetNestedProperty(object target, string propertyPath, object value)
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
        // Handle nested objects (dictionaries)
        if (value is Dictionary<object, object> dict)
        {
            return ConvertNestedObject(targetType, dict);
        }
        // Handle lists/arrays for Vector2, Color, etc.
        else if (value is IList list)
        {
            if (targetType == typeof(Vector2))
            {
                return ParseVector2(list);
            }
            else if (targetType == typeof(Color))
            {
                return ParseColor(list);
            }
            else
            {
                throw new Exception($"Unsupported list conversion to type '{targetType.Name}'");
            }
        }
        // Handle primitive values and other types
        else
        {
            string stringValue = value.ToString() ?? throw new Exception("Value cannot be null");

            // Remove outer quotes if present
            if ((stringValue.StartsWith("\"") && stringValue.EndsWith("\"")) ||
                (stringValue.StartsWith("'") && stringValue.EndsWith("'")))
            {
                stringValue = stringValue.Substring(1, stringValue.Length - 2);
            }

            return targetType switch
            {
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
                _ => throw new Exception($"Unsupported type '{targetType.Name}' for value '{stringValue}'")
            };
        }
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

    private static Vector2 ParseVector2(IList list)
    {
        if (list.Count != 2)
            throw new ArgumentException("Vector2 requires exactly 2 elements");

        float x = Convert.ToSingle(list[0]);
        float y = Convert.ToSingle(list[1]);

        return new Vector2(x, y);
    }

    private static Color ParseColor(IList list)
    {
        if (list.Count < 3 || list.Count > 4)
            throw new ArgumentException("Color requires 3 or 4 elements");

        byte r = Convert.ToByte(list[0]);
        byte g = Convert.ToByte(list[1]);
        byte b = Convert.ToByte(list[2]);
        byte a = list.Count > 3 ? Convert.ToByte(list[3]) : (byte)255;

        return new Color(r, g, b, a);
    }

    private static Audio ParseAudio(string path)
    {
        return new Audio(path);
    }

    private static Texture ParseTexture(string path)
    {
        return TextureManager.Instance.Get(path);
    }
}