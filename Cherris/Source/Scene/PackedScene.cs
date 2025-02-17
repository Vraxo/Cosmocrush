using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class PackedScene(string path)
{
    private readonly string _path = path;
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public T Instantiate<T>(bool isRootNode = false) where T : Node
    {
        // Reset state for new instantiation
        var deferredNodeAssignments = new List<(Node, string, object)>();
        var namedNodes = new Dictionary<string, Node>();

        string yamlContent = File.ReadAllText(_path);
        var rootElement = _deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
        Node rootNode = (T)ParseNode(rootElement, null, deferredNodeAssignments, namedNodes);
        AssignDeferredNodes(deferredNodeAssignments, namedNodes);
        return (T)rootNode;
    }

    private Node ParseNode(
        Dictionary<string, object> element,
        Node? parent,
        List<(Node, string, object)> deferredNodeAssignments,
        Dictionary<string, Node> namedNodes)
    {
        var node = CreateNodeInstance(element);
        ProcessNestedScene(element, ref node);
        SetNodeProperties(element, node, deferredNodeAssignments);
        AddToParent(parent, node);
        RegisterNode(element, node, namedNodes);
        ProcessChildNodes(element, node, deferredNodeAssignments, namedNodes);
        return node;
    }

    private Node CreateNodeInstance(Dictionary<string, object> element)
    {
        var typeName = (string)element["type"];
        var nodeType = PackedSceneUtils.ResolveType(typeName);
        var node = (Node)Activator.CreateInstance(nodeType)!;
        node.Name = (string)element["name"];
        return node;
    }

    private void ProcessNestedScene(Dictionary<string, object> element, ref Node node)
    {
        if (element.TryGetValue("path", out var pathValue))
        {
            var scenePath = (string)pathValue;
            var nestedScene = new PackedScene(scenePath);
            node = nestedScene.Instantiate<Node>();
            node.Name = (string)element["name"];
        }
    }

    private void SetNodeProperties(
        Dictionary<string, object> element,
        Node node,
        List<(Node, string, object)> deferredNodeAssignments)
    {
        var properties = element
            .Where(kvp => !IsReservedKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        PackedSceneUtils.SetProperties(node, properties, deferredNodeAssignments);
    }

    private bool IsReservedKey(string key) =>
        key is "children" or "name" or "type" or "path";

    private void AddToParent(Node? parent, Node node)
    {
        parent?.AddChild(node, node.Name);
    }

    private void RegisterNode(
        Dictionary<string, object> element,
        Node node,
        Dictionary<string, Node> namedNodes)
    {
        var nodeName = (string)element["name"];
        namedNodes[nodeName] = node;
    }

    private void ProcessChildNodes(
        Dictionary<string, object> element,
        Node parentNode,
        List<(Node, string, object)> deferredNodeAssignments,
        Dictionary<string, Node> namedNodes)
    {
        if (!element.TryGetValue("children", out var childrenObj)) return;
        var children = ConvertChildrenToList(childrenObj);
        foreach (var child in children)
        {
            if (child is Dictionary<object, object> childDict)
            {
                var convertedChild = ConvertChildDictionary(childDict);
                ParseNode(convertedChild, parentNode, deferredNodeAssignments, namedNodes);
            }
        }
    }

    private List<object> ConvertChildrenToList(object childrenObj) =>
        childrenObj is List<object> list ? list : new List<object>();

    private Dictionary<string, object> ConvertChildDictionary(Dictionary<object, object> childDict) =>
        childDict.ToDictionary(kvp => kvp.Key.ToString()!, kvp => kvp.Value);

    private void AssignDeferredNodes(
        List<(Node, string, object)> deferredNodeAssignments,
        Dictionary<string, Node> namedNodes)
    {
        foreach (var (targetNode, memberPath, nodePath) in deferredNodeAssignments)
        {
            AssignDeferredNode(targetNode, memberPath, nodePath, namedNodes);
        }
    }

    private void AssignDeferredNode(
        Node targetNode,
        string memberPath,
        object nodePath,
        Dictionary<string, Node> namedNodes)
    {
        string[] pathParts = memberPath.Split('/');
        object currentObject = targetNode;

        for (int i = 0; i < pathParts.Length; i++)
        {
            string part = pathParts[i];
            Type currentType = currentObject.GetType();
            (MemberInfo? memberInfo, object? nextObject) = GetMemberAndNextObject(currentType, part, currentObject);
            if (i == pathParts.Length - 1)
            {
                AssignNodeToMember(memberInfo, currentObject, nodePath, targetNode, namedNodes);
            }
            else
            {
                currentObject = nextObject!;
            }
        }
    }

    private (MemberInfo?, object?) GetMemberAndNextObject(Type type, string memberName, object currentObject)
    {
        PropertyInfo? propertyInfo = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo != null)
        {
            object? nextObject = propertyInfo.GetValue(currentObject) ?? Activator.CreateInstance(propertyInfo.PropertyType);
            propertyInfo.SetValue(currentObject, nextObject);
            return (propertyInfo, nextObject);
        }

        FieldInfo? fieldInfo = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            object? nextObject = fieldInfo.GetValue(currentObject) ?? Activator.CreateInstance(fieldInfo.FieldType);
            fieldInfo.SetValue(currentObject, nextObject);
            return (fieldInfo, nextObject);
        }

        throw new Exception($"Member '{memberName}' not found on type '{type.Name}'.");
    }

    private void AssignNodeToMember(
        MemberInfo? memberInfo,
        object targetObject,
        object nodePath,
        Node targetNode,
        Dictionary<string, Node> namedNodes)
    {
        if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsSubclassOf(typeof(Node)))
        {
            Node referencedNode = ResolveNodePath(nodePath, namedNodes, targetNode);
            propertyInfo.SetValue(targetObject, referencedNode);
        }
        else if (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType.IsSubclassOf(typeof(Node)))
        {
            Node referencedNode = ResolveNodePath(nodePath, namedNodes, targetNode);
            fieldInfo.SetValue(targetObject, referencedNode);
        }
        else
        {
            throw new Exception($"Member '{memberInfo?.Name}' is not a Node-derived type.");
        }
    }

    private Node ResolveNodePath(object nodePath, Dictionary<string, Node> namedNodes, Node targetNode)
    {
        if (nodePath is string pathString)
        {
            if (namedNodes.TryGetValue(pathString, out Node? node))
                return node;

            // Fallback to GetNode if not found in namedNodes
            return targetNode.GetNode<Node>(pathString);
        }
        throw new Exception($"Unsupported node path type: {nodePath.GetType()}");
    }
}