﻿using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class PackedSceneYamlNested(string path)
{
    private readonly string _path = path;
    private readonly Dictionary<string, Node> _namedNodes = new();
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private List<(Node, string, object)> _deferredNodeAssignments = new();

    public T Instantiate<T>(bool isRootNode = false) where T : Node
    {
        var yamlContent = File.ReadAllText(_path);
        var rootElement = _deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
        var rootNode = (T)ParseNode(rootElement, null);

        AssignDeferredNodes();

        return rootNode;
    }

    private Node ParseNode(Dictionary<string, object> element, Node? parent)
    {
        var node = CreateNodeInstance(element);
        ProcessNestedScene(element, ref node);
        SetNodeProperties(element, node);

        // Parent-child relationships are inferred by recursion
        AddToParent(parent, node);

        RegisterNode(element, node);
        ProcessChildNodes(element, node);

        return node;
    }

    private Node CreateNodeInstance(Dictionary<string, object> element)
    {
        var typeName = (string)element["type"];
        var nodeType = PackedSceneUtils.ResolveType(typeName);
        var node = (Node)Activator.CreateInstance(nodeType)!;

        node.Name = (string)element["name"];
        Console.WriteLine($"Created node: {node.Name}, Type: {node.GetType().Name}");
        return node;
    }

    private void ProcessNestedScene(Dictionary<string, object> element, ref Node node)
    {
        if (element.TryGetValue("path", out var pathValue))
        {
            var scenePath = (string)pathValue;
            Console.WriteLine($"Loading nested scene: {scenePath}");
            var nestedScene = new PackedSceneYamlNested(scenePath);
            node = nestedScene.Instantiate<Node>();
        }
    }

    private void SetNodeProperties(Dictionary<string, object> element, Node node)
    {
        var properties = element
            .Where(kvp => !IsReservedKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        Console.WriteLine($"Setting properties for {node.Name}: {string.Join(", ", properties.Keys)}");
        PackedSceneUtils.SetProperties(node, properties, _deferredNodeAssignments);
    }

    private bool IsReservedKey(string key) =>
        key is "children" or "name" or "type" or "path";

    private void AddToParent(Node? parent, Node node)
    {
        if (parent == null)
        {
            Console.WriteLine($"{node.Name} is a root node");
            return;
        }

        Console.WriteLine($"Adding {node.Name} to parent {parent.Name}");
        parent.AddChild(node, node.Name);
    }

    private void RegisterNode(Dictionary<string, object> element, Node node)
    {
        var nodeName = (string)element["name"];
        _namedNodes[nodeName] = node;
        Console.WriteLine($"Registered node: {nodeName}");
    }

    private void ProcessChildNodes(Dictionary<string, object> element, Node parentNode)
    {
        if (!element.TryGetValue("children", out var childrenObj)) return;

        Console.WriteLine($"Processing children for {parentNode.Name}");
        var children = ConvertChildrenToList(childrenObj);

        foreach (var child in children)
        {
            if (child is Dictionary<object, object> childDict)
            {
                var convertedChild = ConvertChildDictionary(childDict);
                ParseNode(convertedChild, parentNode); // Passing the parentNode to maintain the hierarchy
            }
        }
    }

    private List<object> ConvertChildrenToList(object childrenObj)
    {
        if (childrenObj is List<object> list) return list;

        Console.WriteLine($"Unexpected children format: {childrenObj.GetType().Name}");
        return new List<object>();
    }

    private Dictionary<string, object> ConvertChildDictionary(Dictionary<object, object> childDict)
    {
        return childDict.ToDictionary(
            kvp => kvp.Key.ToString()!,
            kvp => kvp.Value
        );
    }

    private void AssignDeferredNodes()
    {
        foreach (var (targetNode, memberPath, nodePath) in _deferredNodeAssignments)
        {
            AssignDeferredNode(targetNode, memberPath, nodePath);
        }
    }

    private void AssignDeferredNode(Node targetNode, string memberPath, object nodePath)
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
                // Final part, assign the Node
                AssignNodeToMember(memberInfo, currentObject, nodePath, targetNode);
            }
            else
            {
                // Intermediate, navigate deeper
                currentObject = nextObject!;
            }
        }
    }

    private (MemberInfo?, object?) GetMemberAndNextObject(Type type, string memberName, object currentObject)
    {
        // Look for a property first (now including NonPublic)
        PropertyInfo? propertyInfo = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo != null)
        {
            object? nextObject = propertyInfo.GetValue(currentObject);
            if (nextObject == null)
            {
                nextObject = Activator.CreateInstance(propertyInfo.PropertyType);
                propertyInfo.SetValue(currentObject, nextObject);
            }
            return (propertyInfo, nextObject);
        }

        // Look for a field if no property is found (now including NonPublic)
        FieldInfo? fieldInfo = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            object? nextObject = fieldInfo.GetValue(currentObject);
            if (nextObject == null)
            {
                nextObject = Activator.CreateInstance(fieldInfo.FieldType);
                fieldInfo.SetValue(currentObject, nextObject);
            }
            return (fieldInfo, nextObject);
        }

        throw new Exception($"Member '{memberName}' not found on type '{type.Name}'.");
    }

    private void AssignNodeToMember(MemberInfo? memberInfo, object targetObject, object nodePath, Node targetNode)
    {
        if (memberInfo is PropertyInfo propertyInfo && propertyInfo.PropertyType.IsSubclassOf(typeof(Node)))
        {
            // Use GetNode to find the referenced Node
            MethodInfo getnodeMethod = targetNode.GetType().GetMethod("GetNode")!.MakeGenericMethod(propertyInfo.PropertyType);
            object? referencedNode = getnodeMethod.Invoke(targetNode, new object[] { nodePath });

            if (referencedNode == null)
            {
                throw new Exception($"Node at path '{nodePath}' not found or is not of type '{propertyInfo.PropertyType.Name}'.");
            }

            propertyInfo.SetValue(targetObject, referencedNode);
        }
        else if (memberInfo is FieldInfo fieldInfo && fieldInfo.FieldType.IsSubclassOf(typeof(Node)))
        {
            // Use GetNode to find the referenced Node
            MethodInfo getnodeMethod = targetNode.GetType().GetMethod("GetNode")!.MakeGenericMethod(fieldInfo.FieldType);
            object? referencedNode = getnodeMethod.Invoke(targetNode, new object[] { nodePath });

            if (referencedNode == null)
            {
                throw new Exception($"Node at path '{nodePath}' not found or is not of type '{fieldInfo.FieldType.Name}'.");
            }

            fieldInfo.SetValue(targetObject, referencedNode);
        }
        else
        {
            throw new Exception($"Member '{memberInfo?.Name}' is not a Node-derived type.");
        }
    }
}