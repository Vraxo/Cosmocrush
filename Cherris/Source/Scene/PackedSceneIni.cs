//using IniParser;
//using IniParser.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//
//namespace Cherris;
//
//public sealed class PackedSceneIni(string path)
//{
//    private readonly string path = path;
//    private readonly Dictionary<string, Node> namedNodes = new();
//    private static readonly FileIniDataParser iniParser = new();
//
//    public T Instantiate<T>(bool isRootNode = false) where T : Node
//    {
//        IniData iniData = iniParser.ReadFile(path);
//        List<Dictionary<string, object>> nodeDataList = ParseIniData(iniData);
//        List<Node> createdNodes = ParseNodeList(nodeDataList);
//        Node? rootNode = createdNodes.FirstOrDefault();
//
//        return (T)rootNode!;
//    }
//
//    private List<Dictionary<string, object>> ParseIniData(IniData iniData)
//    {
//        List<Dictionary<string, object>> nodes = new();
//        foreach (SectionData section in iniData.Sections)
//        {
//            Dictionary<string, object> nodeData = new();
//            nodeData["name"] = section.SectionName;
//            foreach (KeyData keyData in section.Keys)
//            {
//                nodeData[keyData.KeyName] = keyData.Value;
//            }
//            nodes.Add(nodeData);
//        }
//        return nodes;
//    }
//
//    private List<Node> ParseNodeList(List<Dictionary<string, object>> nodes)
//    {
//        List<Node> createdNodes = new();
//
//        // First pass: Create nodes
//        foreach (var element in nodes)
//        {
//            string type = (string)element["type"];
//            string name = (string)element["name"];
//
//            Type nodeType = PackedSceneUtils.ResolveType(type);
//            Node node = (Node)Activator.CreateInstance(nodeType)!;
//
//            // Modified line: Added Trim('"') for path values
//            if (element.TryGetValue("path", out object? value))
//            {
//                string scenePath = ((string)value).Trim('"'); // <- THE CHANGED LINE
//                PackedSceneIni nestedScene = new(scenePath);
//                var nestedRootNode = nestedScene.Instantiate<Node>();
//                node = nestedRootNode;
//            }
//
//            PackedSceneUtils.SetProperties(node, element);
//            namedNodes[name] = node;
//            createdNodes.Add(node);
//        }
//
//        // Second pass: Parent-child relationships
//        foreach (var element in nodes)
//        {
//            string name = (string)element["name"];
//            string? parentName = element.ContainsKey("parent") ? (string?)element["parent"] : null;
//
//            if (parentName != null && namedNodes.TryGetValue(parentName, out Node? parent))
//            {
//                namedNodes[name].Parent = parent;
//                parent.AddChild(namedNodes[name], name);
//            }
//        }
//
//        return createdNodes;
//    }
//}