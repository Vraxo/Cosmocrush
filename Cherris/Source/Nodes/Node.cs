﻿using Spectre.Console;

namespace Cherris;

public class Node
{
    public enum ProcessMode
    {
        Inherit,
        Pausable,
        WhenPaused,
        Disabled,
        Always
    }

    public static Node RootNode => SceneTree.Instance.RootNode!;
    public static SceneTree Tree => SceneTree.Instance;

    public string Name { get; set; } = "";
    public Node? Parent { get; set; } = null;
    public List<Node> Children { get; set; } = [];
    public ProcessMode ProcessingMode = ProcessMode.Inherit;

    public bool Active
    {
        get;

        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            ActiveChanged?.Invoke(this, Active);
        }
    } = true;

    public string AbsolutePath
    {
        get
        {
            if (Parent is null)
            {
                // This is the root node
                return "/root/";
            }

            Stack<string> pathStack = new();
            Node? current = this;

            // Traverse the hierarchy upwards, collecting names
            while (current is not null && current.Parent is not null)
            {
                // Skip the root node's name (Parent will be null for root)
                pathStack.Push(current.Name);
                current = current.Parent;
            }

            // Build the absolute Pathetic
            return $"/root/{string.Join("/", pathStack)}";
        }
    }

    public bool IsReady = false;

    // Events

    public delegate void ActiveEvent(Node sender, bool active);
    public delegate void ChildEvent(Node sender, Node child);
    public event ActiveEvent? ActiveChanged;
    public event ChildEvent? ChildAdded;

    // Other

    public virtual void Make() { }

    public virtual void Start() { }

    public virtual void Ready() { }

    public virtual void Free()
    {
        List<Node> childrenToDestroy = new(Children);

        foreach (Node child in childrenToDestroy)
        {
            child.Free();
        }

        Parent?.Children.Remove(this);
    }

    public virtual void ProcessBegin() { }

    public virtual void Process() { }

    public virtual void ProcessEnd() { }

    public void PrintChildren()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Create root tree node with emoji based on root node type
        string rootEmoji = NodeEmoji.GetEmojiForNodeType(this);
        Tree root = new($"{rootEmoji}[green]{Name}[/]");

        // Add children nodes recursively, each with its own emoji
        AddChildrenToTree(this, root);

        // Print the tree structure using Spectre.Console
        AnsiConsole.Write(root);
    }

    private static void AddChildrenToTree(Node node, IHasTreeNodes parentNode)
    {
        foreach (var child in node.Children)
        {
            // Get emoji for each child node based on its type
            string childEmoji = NodeEmoji.GetEmojiForNodeType(child);

            // Add the child node to the tree with the emoji
            TreeNode childNode = parentNode.AddNode($"{childEmoji} [blue]{child.Name}[/]");

            // Recursively add children to this child node
            AddChildrenToTree(child, childNode);
        }
    }

    public virtual void Activate()
    {
        Active = true;

        foreach (Node child in Children)
        {
            child.Activate();
        }
    }

    public virtual void Deactivate()
    {
        Active = false;

        foreach (Node child in Children)
        {
            child.Deactivate();
        }
    }

    public T GetParent<T>() where T : Node
    {
        if (Parent is not null)
        {
            return (T)Parent;
        }

        return (T)this;
    }
    
    public T GetNode<T>(string path) where T : Node
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Pathetic cannot be null or empty.", nameof(path));
        }

        Node? currentNode;

        if (path.StartsWith("/root"))
        {
            path = path["/root".Length..];
            currentNode = SceneTree.Instance.RootNode;

            if (path.StartsWith('/'))
            {
                path = path.Substring(1);
            }

            if (!string.IsNullOrEmpty(path))
            {
                string[] nodeNames = path.Split('/');
                foreach (var name in nodeNames)
                {
                    if (name == "..")
                    {
                        // Traverse back to parent
                        currentNode = currentNode?.Parent;
                    }
                    else
                    {
                        currentNode = currentNode?.GetChild(name);
                    }

                    if (currentNode == null)
                    {
                        throw new InvalidOperationException($"Node '{name}' not found in the scene tree.");
                    }
                }
            }
        }
        else
        {
            currentNode = this;
            string[] nodeNames = path.Split('/');
            foreach (var name in nodeNames)
            {
                if (name == "..")
                {
                    // Traverse back to parent
                    currentNode = currentNode?.Parent;
                }
                else if (name != "")
                {
                    currentNode = currentNode?.GetChild(name);
                }

                if (currentNode == null)
                {
                    throw new InvalidOperationException($"Node '{name}' not found in the scene tree.");
                }
            }
        }

        return currentNode as T ?? throw new InvalidOperationException("Node is not of the expected type.");
    }
    
    public T? GetNodeOrNull<T>(string path) where T : Node
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        Node? currentNode;

        if (path.StartsWith("/root"))
        {
            path = path.Substring("/root".Length);
            currentNode = SceneTree.Instance.RootNode;

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            if (!string.IsNullOrEmpty(path))
            {
                string[] nodeNames = path.Split('/');
                foreach (var name in nodeNames)
                {
                    if (name == "..")
                    {
                        // Traverse back to parent
                        currentNode = currentNode?.Parent;
                    }
                    else
                    {
                        currentNode = currentNode?.GetChild(name);
                    }

                    if (currentNode == null)
                    {
                        return null;
                    }
                }
            }
        }
        else
        {
            currentNode = this;
            string[] nodeNames = path.Split('/');
            foreach (var name in nodeNames)
            {
                if (name == "..")
                {
                    // Traverse back to parent
                    currentNode = currentNode?.Parent;
                }
                else if (name != "")
                {
                    currentNode = currentNode?.GetChildOrNull(name);
                }

                if (currentNode == null)
                {
                    return null;
                }
            }
        }

        return currentNode as T;
    }

    public T? GetChild<T>(string name) where T : Node
    {
        foreach (Node child in Children)
        {
            if (child.Name == name)
            {
                return (T)child;
            }
        }

        return null;
    }
    
    public T? GetChild<T>() where T : Node
    {
        foreach (Node child in Children)
        {
            if (child.GetType() == typeof(T))
            {
                return (T)child;
            }
        }

        return null;
    }

    public Node GetChild(string name)
    {
        foreach (Node child in Children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }

        SceneTree.Instance.RootNode?.PrintChildren();

        throw new InvalidOperationException($"Child node with name '{name}' not found.");
    }

    public Node? GetChildOrNull(string name)
    {
        foreach (Node child in Children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }

        return null;
    }

    public Node AddChild(Node node, string name = "")
    {
        node.Name = name;
        node.Parent = this;

        node.Make();

        Children.Add(node);
        ChildAdded?.Invoke(this, node);

        return node;
    }
}