using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public partial class ThemeManager : Node
{
    [ExportGroup("UI Elements")]
    [Export] Node[] UINodes;

    [ExportGroup("Customization")]
    [Export] public Color backgroundColor;
    [Export] public Color accentColor;

    [ExportGroup("Hardcoded")]
    [Export] StyleBoxFlat buttonStylebox;
    [Export] StyleBoxFlat toggleStylebox;

    List<Node> nodes = [];
    public Color backgroundAccent;

    public static ThemeManager Instance { get; private set; }
    public override void _Ready()
    {
        //singleton
        Instance = this;

        //get all original nodes
        foreach (var node in UINodes)
        {
            nodes.Add(node);

            if (node.GetChildCount() != 0)
            {
                nodes.AddRange(GetChildrenRecursive(node));
            }
        }

        SetTheme();
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

    public void SetTheme()
	{
        //calculate secondary pallete
        backgroundAccent = CombineColors(backgroundColor, accentColor, 0.15f);

        //apply theme
        foreach (var node in nodes)
        {
            ApplyTheme(node);
        }
    }

    public void ApplyTheme(Node startNode, bool recursive = false)
    {
        List<Node> applyNodes = [startNode];
        if (recursive) applyNodes.AddRange(GetChildrenRecursive(startNode));

        for (int i = 0; i < applyNodes.Count; i++)
        {
            var node = applyNodes[i];
            if (!node.HasMeta("ui")) continue;

            string style = (string)node.GetMeta("ui");

            if (node is Panel panel)
            {
                var stylebox = (StyleBoxFlat)panel.GetThemeStylebox("panel");

                if (style == "background")
                {
                    //keep things like rounded edges intact
                    stylebox.BgColor = backgroundColor;
                    panel.AddThemeStyleboxOverride("panel", stylebox);
                }
                else if (style == "background_accent")
                {
                    //keep things like rounded edges intact
                    stylebox.BgColor = backgroundAccent;
                    panel.AddThemeStyleboxOverride("panel", stylebox);
                }
                else if (style == "accent")
                {
                    //keep things like rounded edges intact
                    stylebox.BgColor = accentColor;
                    panel.AddThemeStyleboxOverride("panel", stylebox);
                }
            }
            else if (node is Button button)
            {
                if (style == "button")
                {
                    //create new stylebox, keep things like rounded edges intact
                    Color buttonColor = accentColor;
                    StyleBoxFlat normalBox = CreateStylebox(buttonStylebox, buttonColor);
                    button.AddThemeStyleboxOverride("normal", normalBox);

                    buttonColor = buttonColor.Lightened(0.1f);
                    StyleBoxFlat hoverBox = CreateStylebox(buttonStylebox, buttonColor);
                    button.AddThemeStyleboxOverride("hover", hoverBox);

                    buttonColor = buttonColor.Lightened(0.2f);
                    StyleBoxFlat pressedBox = CreateStylebox(buttonStylebox, buttonColor);
                    button.AddThemeStyleboxOverride("pressed", pressedBox);

                }
                else if (style == "toggle")
                {
                    button.AddThemeStyleboxOverride("normal", CreateStyleboxEmpty(toggleStylebox));

                    Color toggleColor = accentColor;
                    toggleColor.A = 0.5f;
                    StyleBoxFlat pressedBox = CreateStylebox(toggleStylebox, toggleColor);
                    button.AddThemeStyleboxOverride("pressed", pressedBox);

                    toggleColor.A = 0.15f;
                    StyleBoxFlat hoverBox = CreateStylebox(toggleStylebox, toggleColor);
                    button.AddThemeStyleboxOverride("hover", hoverBox);

                }
            }
        }
    }

    public Color CombineColors(Color color1, Color color2, float weight = 0.5f)
    {
        return new()
        {
            R = Mathf.Lerp(color1.R, color2.R, weight),
            G = Mathf.Lerp(color1.G, color2.G, weight),
            B = Mathf.Lerp(color1.B, color2.B, weight),
            A = Mathf.Lerp(color1.A, color2.A, weight)
        };
    }

    public StyleBoxFlat CreateStylebox(StyleBoxFlat box, Color color)
    {
        return new()
        {
            BgColor = color,
            DrawCenter = box.DrawCenter,
            Skew = box.Skew,
            BorderWidthBottom = box.BorderWidthBottom,
            BorderWidthLeft = box.BorderWidthLeft,
            BorderWidthRight = box.BorderWidthRight,
            BorderWidthTop = box.BorderWidthTop,
            BorderBlend = box.BorderBlend,
            BorderColor = box.BorderColor,
            CornerRadiusBottomLeft = box.CornerRadiusBottomLeft,
            CornerRadiusBottomRight = box.CornerRadiusBottomRight,
            CornerRadiusTopLeft = box.CornerRadiusTopLeft,
            CornerRadiusTopRight = box.CornerRadiusTopRight,
            CornerDetail = box.CornerDetail,
            ExpandMarginBottom = box.ExpandMarginBottom,
            ExpandMarginLeft = box.ExpandMarginLeft,
            ExpandMarginRight = box.ExpandMarginRight,
            ExpandMarginTop = box.ExpandMarginTop,
            ShadowColor = box.ShadowColor,
            ShadowOffset = box.ShadowOffset,
            ShadowSize = box.ShadowSize,
            AntiAliasing = box.AntiAliasing,
            AntiAliasingSize = box.AntiAliasingSize,
            ContentMarginBottom = box.ContentMarginBottom,
            ContentMarginLeft = box.ContentMarginLeft,
            ContentMarginRight = box.ContentMarginRight,
            ContentMarginTop = box.ContentMarginTop,
        };
    }

    public StyleBoxEmpty CreateStyleboxEmpty(StyleBoxFlat box)
    {
        return new()
        {
            ContentMarginBottom = box.ContentMarginBottom,
            ContentMarginLeft = box.ContentMarginLeft,
            ContentMarginRight = box.ContentMarginRight,
            ContentMarginTop = box.ContentMarginTop
        };
    }
}
