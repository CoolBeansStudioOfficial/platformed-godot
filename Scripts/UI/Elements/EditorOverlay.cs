using Godot;
using System;

public partial class EditorOverlay : Control
{
    public Outline currentOutline;

    public void SetOutline(Outline outline)
    {
        currentOutline = outline;
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawRect(currentOutline.rect, currentOutline.color, false, currentOutline.width, true);
    }

    public struct Outline
    {
        public Rect2 rect;
        public Color color;
        public float width;
    }
}
