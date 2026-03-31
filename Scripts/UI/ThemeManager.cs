using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class ThemeManager : Node
{
    [Export] Node[] UINodes;

    public class ColorTheme
    {
        [JsonPropertyName("color")]
        public Color color;
    }

    public static ThemeManager Instance { get; private set; }
    public override void _Ready()
    {
        //singleton
        Instance = this;

        SetTheme();
    }

    public void SetTheme()
	{
        List<Node> nodes = [];
        foreach (var node in UINodes)
        {
            nodes.Add(node);

            if (node.GetChildCount() != 0)
            {
                nodes.AddRange(GetChildrenRecursive(node));
            }
        }

        foreach (var node in nodes)
        {
            GD.Print(node.Name);
        }
    }

    List<Node> GetChildrenRecursive(Node node)
    {
        if (node.GetChildCount() == 0) return null;

        List<Node> children = [];

        foreach (var child in node.GetChildren())
        {
            children.Add(child);

            if (child.GetChildCount() != 0)
            {
                children.AddRange(GetChildrenRecursive(child));
            }
        }

        return children;
    }
}
