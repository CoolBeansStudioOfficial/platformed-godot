using Godot;
using System;

public partial class EditorOverlay : Control
{
    public Outline? currentOutline;
    public Outline? currentTexture;

    public void SetOutline(Outline? outline)
    {
        currentOutline = outline;
        QueueRedraw();
    }

    public void SetTexture(Outline? texture)
    {
        currentTexture = texture;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (currentOutline.HasValue)
        {
            DrawRect(currentOutline.Value.rect, currentOutline.Value.color, false, currentOutline.Value.width, true);
        }
        
        if (currentTexture.HasValue)
        {
            DrawTextureRectRegion(currentTexture.Value.texture, currentTexture.Value.rect, currentTexture.Value.region, currentTexture.Value.color);
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
