using Godot;
using System;
using System.Threading;

public partial class TilesViewport : ScrollContainer
{
    [Export] public Vector2 gridSize;

    [Export] float minZoom;
    [Export] float maxZoom;
    [Export] float zoomSpeed;
    [Export] Control content;
    [Export] Control grid;

    float zoom = 1f;
    bool dragging = false;
    Vector2 lastMousePosition = Vector2.Zero;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Middle)
            {
                if (mb.Pressed)
                {
                    dragging = true;
                    MouseDefaultCursorShape = CursorShape.Drag;

                    lastMousePosition = mb.Position;
                    GD.Print("middle mouse button pressed in viewport");

                }
                else
                {
                    dragging = false;
                    MouseDefaultCursorShape = CursorShape.Arrow;
                    GD.Print("middle mouse button released in viewport");
                }
            }
            else if (mb.ButtonIndex == MouseButton.WheelUp && mb.AltPressed)
            {
                Zoom(zoomSpeed);

                AcceptEvent();
            }
            else if (mb.ButtonIndex == MouseButton.WheelDown && mb.AltPressed)
            {
                Zoom(-zoomSpeed);

                AcceptEvent();
            }
        }
        else if (@event is InputEventMouseMotion m)
        {
            if (!dragging) return;

            Vector2 delta = m.Position - lastMousePosition;
            lastMousePosition = m.Position;

            ScrollHorizontal -= (int)delta.X;
            ScrollVertical -= (int)delta.Y;
        }
    }

    public void Zoom(float amount)
    {
        zoom += amount;

        if (zoom < minZoom) zoom = minZoom;
        else if (zoom > maxZoom) zoom = maxZoom;
    }

    public override void _Process(double delta)
    {
        Vector2 newZoom = new(zoom, zoom);
        content.CustomMinimumSize = gridSize * zoom;
        grid.Scale = newZoom;
    }
}
