/*using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cherris;

public sealed class PackedSceneYaml(string path)
{
    private readonly string path = path;
    private readonly Dictionary<string, Node> namedNodes = new();
    private static readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public T Instantiate<T>(bool isRootNode = false) where T : Node
    {
        string yamlString = File.ReadAllText(path);
        var nodes = deserializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(yamlString);

        // Retrieve the root node (the first element in the dictionary)
        Node rootNode = ParseNodeList(nodes, null);

        return (T)rootNode!;
    }

    private Node ParseNodeList(Dictionary<string, Dictionary<string, object>> nodes, Node? parent)
    {
        Node? lastNode = null;

        foreach (var element in nodes)
        {
            string type = (string)element.Value["type"];
            string name = element.Key;  // Using the key (e.g., "Particles", "MainMenu", "Background")

            // Infer parentName directly from nesting (we no longer use the "parent" key)
            string? parentName = parent?.Name; // Parent name inferred from the parent node

            Type nodeType = PackedSceneUtils.ResolveType(type);
            Node node = (Node)Activator.CreateInstance(nodeType)!;

            // Handling 'path' for nested scenes
            if (element.Value.TryGetValue("path", out object? value))
            {
                string scenePath = (string)value;
                PackedSceneYaml nestedScene = new(scenePath);
                var nestedRootNode = nestedScene.Instantiate<Node>();
                node = nestedRootNode;
            }

            // Set properties for the node from the YAML element
            PackedSceneUtils.SetProperties(node, element.Value);

            // Add child nodes (including handling parent-child relationships)
            if (parent == null)
            {
                // For the root node (no parent)
                lastNode = node;
            }
            else
            {
                // Add this node as a child to the parent
                parent.AddChild(node, name);
            }

            // Process children (recursive nesting)
            if (element.Value.ContainsKey("children"))
            {
                var children = (List<object>)element.Value["children"];
                foreach (var childElement in children)
                {
                    var childDict = (Dictionary<string, object>)childElement;
                    // Recursively parse the child elements
                    ParseNodeList(new Dictionary<string, Dictionary<string, object>> { { childDict["name"].ToString(), (Dictionary<string, object>)childDict } }, node);
                }
            }

            namedNodes[name] = node;
        }

        return lastNode!;
    }
}
*/