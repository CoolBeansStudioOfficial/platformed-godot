using Godot;
using System;
using System.Collections.Generic;

public partial class EditorOverlay : Control
{
    public List<Outline?> currentOutlines = [default, default, default];
    public List<Outline?> currentTextures = [];

    public void SetOutline(Outline? outline, int index = 0)
    {
        currentOutlines[index] = outline;
        QueueRedraw();
    }

    public void SetTextures(List<Outline?> textures)
    {
        currentTextures = textures;
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (var outline in currentOutlines)
        {
            if (outline.HasValue)
            {
                DrawRect(outline.Value.rect, outline.Value.color, false, outline.Value.width, true);
            }
        }
        
        if (currentTextures is not null) foreach (var texture in currentTextures)
        {
            if (texture.HasValue)
            {
                DrawTextureRectRegion(texture.Value.texture, texture.Value.rect, texture.Value.region, texture.Value.color);
            }
        }
        
    }

    public struct Outline
    {
        public Texture2D texture;
        public Rect2 rect;
        public Rect2 region;
        public Color color;
        public float width;
    }
}
