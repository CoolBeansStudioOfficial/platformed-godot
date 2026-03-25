using Godot;
using System;

public partial class EditorOverlay : Control
{
    public Outline currentOutline;
    public Outline? currentTexture;

    public void SetOutline(Outline outline)
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
        DrawRect(currentOutline.rect, currentOutline.color, false, currentOutline.width, true);
        if (currentTexture.HasValue)
        {
            GD.Print("should be drawing texture");
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
