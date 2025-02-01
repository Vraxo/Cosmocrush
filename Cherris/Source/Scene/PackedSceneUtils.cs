using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cherris;

public static class PackedSceneUtils
{
    private static readonly string[] SpecialProperties = { "type", "name", "path" };

    public static void SetProperties(Node node, Dictionary<string, object> element, List<(Node, string, object)>? deferredNodeAssignments = null)
    {
        foreach (KeyValuePair<string, object> member in element)
        {
            string memberName = member.Key;
            if (SpecialProperties.Contains(memberName))
            {
                continue;
            }

            object value = member.Value;
            SetNestedMember(node, memberName, value, deferredNodeAssignments);
        }
    }

    public static void SetNestedMember(object target, string memberPath, object value, List<(Node, string, object)>? deferredNodeAssignments = null)
    {
        string[] pathParts = memberPath.Split('/');
        object currentObject = target;

        for (int i = 0; i < pathParts.Length; i++)
        {
            string part = pathParts[i];

            // Get the Type of the current object
            Type currentType = currentObject.GetType();

            // Look for a property first (now including NonPublic)
            PropertyInfo? propertyInfo = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                if (i == pathParts.Length - 1)
                {
                    // Final part of the path, handle value assignment or defer if Node type
                    if (propertyInfo.PropertyType.IsSubclassOf(typeof(Node)) && value is string nodePath)
                    {
                        // Defer assignment of Node types
                        deferredNodeAssignments?.Add(((Node)target, memberPath, value));
                    }
                    else
                    {
                        // Set the value for non-Node types or non-string values
                        object propertyValue = ConvertValue(propertyInfo.PropertyType, value);
                        propertyInfo.SetValue(currentObject, propertyValue);
                    }
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
            else
            {
                // Look for a field if no property is found (now including NonPublic)
                FieldInfo? fieldInfo = currentType.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    if (i == pathParts.Length - 1)
                    {
                        // Final part of the path, handle value assignment or defer if Node type
                        if (fieldInfo.FieldType.IsSubclassOf(typeof(Node)) && value is string nodePath)
                        {
                            // Defer assignment of Node types
                            deferredNodeAssignments?.Add(((Node)target, memberPath, value));
                        }
                        else
                        {
                            // Set the value for non-Node types or non-string values
                            object fieldValue = ConvertValue(fieldInfo.FieldType, value);
                            fieldInfo.SetValue(currentObject, fieldValue);
                        }
                    }
                    else
                    {
                        // Intermediate part of the path, navigate or create nested object
                        object? nextObject = fieldInfo.GetValue(currentObject);
                        if (nextObject == null)
                        {
                            nextObject = Activator.CreateInstance(fieldInfo.FieldType);
                            fieldInfo.SetValue(currentObject, nextObject);
                        }
                        currentObject = nextObject!;
                    }
                }
                else
                {
                    throw new Exception($"Member '{part}' not found on type '{currentType.Name}'.");
                }
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
                Type t when t == typeof(SoundEffect) => ResourceLoader.Load<SoundEffect>(stringValue),
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

            // Look for a property first (now including NonPublic)
            var propertyInfo = targetType.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                object convertedValue = ConvertValue(propertyInfo.PropertyType, value);
                propertyInfo.SetValue(targetObject, convertedValue);
            }
            else
            {
                // Look for a field if no property is found (now including NonPublic)
                var fieldInfo = targetType.GetField(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    object convertedValue = ConvertValue(fieldInfo.FieldType, value);
                    fieldInfo.SetValue(targetObject, convertedValue);
                }
                else
                {
                    throw new Exception($"Member '{key}' not found on type '{targetType.Name}'.");
                }
            }
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