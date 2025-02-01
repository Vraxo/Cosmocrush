using Cherris;

public static class PropertyRegistry
{
    private static readonly Dictionary<Type, Dictionary<string, Action<Node, object>>> _registry = new();

    public static void Register(Type type, Dictionary<string, Action<Node, object>> setters)
    {
        _registry[type] = setters;
    }

    public static Action<Node, object> GetSetter(Type type, string propertyName)
    {
        Type currentType = type;
        while (currentType != null)
        {
            if (_registry.TryGetValue(currentType, out var setters) && setters.TryGetValue(propertyName, out var setter))
            {
                return setter;
            }
            currentType = currentType.BaseType;
        }
        return null;
    }
}