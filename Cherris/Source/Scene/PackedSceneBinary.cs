using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;

namespace Cherris;

public sealed class PackedSceneBinary
{
    private readonly string _path;
    private readonly Dictionary<string, Node> _namedNodes = new();
    private readonly List<(Node, string, object)> _deferredNodeAssignments = new();

    public PackedSceneBinary(string path) => _path = path;

    private static readonly MessagePackSerializerOptions _options =
        MessagePackSerializerOptions.Standard
            .WithResolver(CompositeResolver.Create(
                NativeDateTimeResolver.Instance,
                StandardResolverAllowPrivate.Instance,
                DynamicEnumResolver.Instance,
                DynamicGenericResolver.Instance,
                DynamicUnionResolver.Instance,
                DynamicObjectResolver.Instance
            ));

    // Rest of the class remains the same, but change the deserialization line:
    public T Instantiate<T>() where T : Node
    {
        byte[] bytes = File.ReadAllBytes(_path);
        var rootElement = MessagePackSerializer.Deserialize<Dictionary<string, object>>(bytes, _options);
        var rootNode = (T)ParseNode(rootElement, null);
        AssignDeferredNodes();
        return rootNode;
    }

    private Node ParseNode(Dictionary<string, object> element, Node? parent)
    {
        var node = CreateNodeInstance(element);
        ProcessNestedScene(element, ref node);
        SetNodeProperties(element, node);
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
        return node;
    }

    private void ProcessNestedScene(Dictionary<string, object> element, ref Node node)
    {
        if (element.TryGetValue("path", out var pathValue))
        {
            var scenePath = (string)pathValue;
            var nestedScene = new PackedSceneBinary(scenePath);
            node = nestedScene.Instantiate<Node>();
            node.Name = (string)element["name"];
        }
    }

    private void SetNodeProperties(Dictionary<string, object> element, Node node)
    {
        var properties = element
            .Where(kvp => !IsReservedKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        PackedSceneUtils.SetProperties(node, properties, _deferredNodeAssignments);
    }

    private bool IsReservedKey(string key) =>
        key is "children" or "name" or "type" or "path";

    private void AddToParent(Node? parent, Node node)
    {
        parent?.AddChild(node, node.Name);
    }

    private void RegisterNode(Dictionary<string, object> element, Node node)
    {
        _namedNodes[(string)element["name"]] = node;
    }

    private void ProcessChildNodes(Dictionary<string, object> element, Node parent)
    {
        if (!element.TryGetValue("children", out var children)) return;

        if (children is List<object> childList)
        {
            foreach (var child in childList)
            {
                if (child is Dictionary<string, object> childDict)
                {
                    ParseNode(childDict, parent);
                }
            }
        }
    }

    private void AssignDeferredNodes()
    {
        foreach (var (targetNode, memberPath, nodePath) in _deferredNodeAssignments)
        {
            PackedSceneUtils.SetNestedMember(
                targetNode,
                memberPath,
                _namedNodes[(string)nodePath]
            );
        }
    }
}