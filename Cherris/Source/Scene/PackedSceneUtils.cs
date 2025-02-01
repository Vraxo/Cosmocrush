using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Cherris;

public static class PackedSceneUtils
{
    private const BindingFlags MemberBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly string[] SpecialProperties = { "type", "name", "path" };

    public static void SetProperties(Node node, Dictionary<string, object> properties, List<(Node, string, object)> deferredNodeAssignments)
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        foreach (var kvp in properties)
        {
            Stopwatch swProp = Stopwatch.StartNew();
            var setter = PropertyRegistry.GetSetter(node.GetType(), kvp.Key);
            if (setter != null)
            {
                setter(node, kvp.Value);
                swProp.Stop();
                Console.WriteLine($"[Property Set] '{kvp.Key}' on node '{node.Name}' in {swProp.Elapsed.TotalMilliseconds:F3} ms");
            }
            else
            {
                // Handle deferred node assignments or other cases
                if (kvp.Value is string pathValue)
                {
                    deferredNodeAssignments.Add((node, kvp.Key, pathValue));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[Deferred] Property '{kvp.Key}' on node '{node.Name}' deferred for later assignment (path: '{pathValue}').");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        swTotal.Stop();
        Console.WriteLine($"[Total Properties] Finished setting properties for node '{node.Name}' in {swTotal.Elapsed.TotalMilliseconds:F3} ms");
    }

    public static void SetNestedMember(object target, string memberPath, object value,
        List<(Node, string, object)>? deferredNodeAssignments = null)
    {
        Stopwatch swNested = Stopwatch.StartNew();
        var pathParts = memberPath.Split('/');
        object currentObject = target;

        for (var i = 0; i < pathParts.Length; i++)
        {
            var memberInfo = GetMemberInfo(currentObject.GetType(), pathParts[i]);
            var isFinalSegment = i == pathParts.Length - 1;

            if (isFinalSegment)
            {
                HandleFinalSegment(target, memberPath, currentObject, memberInfo, value, deferredNodeAssignments);
            }
            else
            {
                currentObject = GetOrCreateIntermediateObject(currentObject, memberInfo);
            }
        }

        swNested.Stop();
        Console.WriteLine($"[Nested Member] Finished processing nested member '{memberPath}' in {swNested.Elapsed.TotalMilliseconds:F3} ms");
    }

    private static MemberInfo GetMemberInfo(Type type, string memberName)
    {
        var property = type.GetProperty(memberName, MemberBindingFlags);
        if (property != null) return property;

        var field = type.GetField(memberName, MemberBindingFlags);
        if (field != null) return field;

        throw new InvalidOperationException($"Member '{memberName}' not found on type '{type.Name}'");
    }

    private static void HandleFinalSegment(object rootTarget, string memberPath, object currentObject,
        MemberInfo memberInfo, object value, List<(Node, string, object)>? deferredAssignments)
    {
        Stopwatch swFinal = Stopwatch.StartNew();
        var memberType = memberInfo switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new InvalidOperationException("Unsupported member type")
        };

        if (ShouldDeferAssignment(memberType, value))
        {
            deferredAssignments?.Add(((Node)rootTarget, memberPath, value));
            Console.WriteLine($"[Deferred Nested] Deferring assignment for nested member '{memberPath}' on node '{((Node)rootTarget).Name}'");
        }
        else
        {
            var convertedValue = ConvertValue(memberType, value);
            SetMemberValue(currentObject, memberInfo, convertedValue);

            if (rootTarget is Node node)
            {
                Console.WriteLine($"[Nested Set] Nested member '{memberPath}' set on node '{node.Name}'");
            }
            else
            {
                Console.WriteLine($"[Nested Set] Nested member '{memberPath}' set on target of type '{rootTarget.GetType().Name}'");
            }
        }
        swFinal.Stop();
        Console.WriteLine($"[HandleFinalSegment] Completed in {swFinal.Elapsed.TotalMilliseconds:F3} ms");
    }

    private static bool ShouldDeferAssignment(Type memberType, object value)
        => memberType.IsSubclassOf(typeof(Node)) && value is string;

    private static object GetOrCreateIntermediateObject(object currentObject, MemberInfo memberInfo)
    {
        var existingValue = GetMemberValue(currentObject, memberInfo);
        if (existingValue != null) return existingValue;

        var newInstance = CreateMemberInstance(memberInfo);
        SetMemberValue(currentObject, memberInfo, newInstance);
        Console.WriteLine($"[Intermediate] Created new instance for member '{memberInfo.Name}' of type '{newInstance.GetType().Name}'.");
        return newInstance;
    }

    private static object CreateMemberInstance(MemberInfo memberInfo)
        => Activator.CreateInstance(memberInfo switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new InvalidOperationException("Unsupported member type")
        }) ?? throw new InvalidOperationException("Failed to create instance");

    private static object? GetMemberValue(object target, MemberInfo memberInfo)
    {
        return memberInfo switch
        {
            PropertyInfo p => p.GetValue(target),
            FieldInfo f => f.GetValue(target),
            _ => throw new InvalidOperationException("Unsupported member type")
        };
    }

    private static void SetMemberValue(object target, MemberInfo memberInfo, object value)
    {
        switch (memberInfo)
        {
            case PropertyInfo p:
                p.SetValue(target, value);
                break;
            case FieldInfo f:
                f.SetValue(target, value);
                break;
            default:
                throw new InvalidOperationException("Unsupported member type");
        }
    }

    public static Type ResolveType(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(typeName, false) ?? assembly.GetType($"{assembly.GetName().Name}.{typeName}", false))
            .FirstOrDefault(type => type != null)
            ?? throw new TypeLoadException($"Type '{typeName}' not found");
    }

    public static object ConvertValue(Type targetType, object value)
    {
        return value switch
        {
            Dictionary<object, object> dict => ConvertNestedObject(targetType, dict),
            IList list => ConvertList(targetType, list),
            _ => ConvertPrimitive(targetType, value)
        };
    }

    private static object ConvertNestedObject(Type targetType, Dictionary<object, object> dict)
    {
        var instance = Activator.CreateInstance(targetType)
            ?? throw new InvalidOperationException($"Failed to create {targetType.Name} instance");

        foreach (var (key, value) in dict)
        {
            var memberName = key.ToString() ?? throw new InvalidDataException("Dictionary key cannot be null");
            var memberInfo = GetMemberInfo(targetType, memberName);

            var convertedValue = ConvertValue(GetMemberType(memberInfo), value);
            SetMemberValue(instance, memberInfo, convertedValue);
        }

        return instance;
    }

    private static Type GetMemberType(MemberInfo memberInfo)
        => memberInfo switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new InvalidOperationException("Unsupported member type")
        };

    private static object ConvertList(Type targetType, IList list)
    {
        // Check if it's a Color type and parse the list into a Color
        if (targetType == typeof(Color))
        {
            return ParseColor(list);
        }

        // Handle other types
        return targetType.Name switch
        {
            nameof(Vector2) => ParseVector2(list),
            _ => throw new NotSupportedException($"Unsupported list conversion to {targetType.Name}")
        };
    }

    private static object ConvertPrimitive(Type targetType, object value)
    {
        var stringValue = TrimQuotes(value.ToString()!)
            ?? throw new InvalidOperationException("Value cannot be null");

        if (targetType.IsEnum)
            return Enum.Parse(targetType, stringValue);

        return targetType switch
        {
            _ when targetType == typeof(Audio) => new Audio(stringValue),
            _ when targetType == typeof(Texture) => TextureManager.Instance.Get(stringValue),
            _ when targetType == typeof(Font) => ResourceLoader.Load<Font>(stringValue),
            _ when targetType == typeof(int) => int.Parse(stringValue),
            _ when targetType == typeof(uint) => uint.Parse(stringValue),
            _ when targetType == typeof(float) => float.Parse(stringValue),
            _ when targetType == typeof(double) => double.Parse(stringValue),
            _ when targetType == typeof(bool) => bool.Parse(stringValue),
            _ when targetType == typeof(string) => stringValue,
            _ when targetType == typeof(Color) => ParseColor(stringValue),
            _ => throw new NotSupportedException($"Unsupported type: {targetType.Name}")
        };
    }

    private static Vector2 ParseVector2(IList list)
    {
        if (list.Count != 2) throw new ArgumentException("Vector2 requires exactly 2 elements");
        return new Vector2(
            Convert.ToSingle(list[0]),
            Convert.ToSingle(list[1])
        );
    }

    private static Color ParseColor(IList list)
    {
        if (list.Count < 3 || list.Count > 4)
            throw new ArgumentException("Color requires 3 or 4 elements");
        return new Color(
            Convert.ToByte(list[0]),
            Convert.ToByte(list[1]),
            Convert.ToByte(list[2]),
            list.Count > 3 ? Convert.ToByte(list[3]) : (byte)255
        );
    }

    private static Color ParseColor(string stringValue)
    {
        // For debugging purposes, you can output the string value.
        Console.WriteLine($"[ParseColor] Converting string '{stringValue}' to Color. (Defaulting to White)");
        return Color.White;
    }

    private static string TrimQuotes(this string input)
        => input.Length >= 2 && (input[0] == '"' || input[0] == '\'')
            ? input.Substring(1, input.Length - 2)
            : input;
}
