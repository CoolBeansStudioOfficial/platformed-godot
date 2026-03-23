using Godot;
using System;
using System.Threading;

public partial class TilesViewport : ScrollContainer
{
    [Export] float minZoom;
    [Export] float maxZoom;
    [Export] float zoomSpeed;
    [Export] TileMapLayer tileMapLayer;
    [Export] Control content;
    [Export] Control grid;

    public Vector2 viewportSize;

    public float zoom = 1f;
    //number of inputs each action has been performed for
    public bool middleDragging = false;
    Vector2 lastMousePosition = Vector2.Zero;

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Middle)
            {
                if (mb.Pressed)
                {
                    middleDragging = true;
                    MouseDefaultCursorShape = CursorShape.Drag;

                    lastMousePosition = mb.Position;
                    GD.Print("middle mouse button pressed in viewport");

                }
                else
                {
                    middleDragging = false;
                    MouseDefaultCursorShape = CursorShape.Arrow;
                    GD.Print("middle mouse button released in viewport");
                }
            }
            else if (mb.ButtonIndex == MouseButton.WheelUp && mb.IsCommandOrControlPressed())
            {
                Zoom(zoomSpeed);

                AcceptEvent();
            }
            else if (mb.ButtonIndex == MouseButton.WheelDown && mb.IsCommandOrControlPressed())
            {
                Zoom(-zoomSpeed);

                AcceptEvent();
            }
        }
        else if (@event is InputEventMouseMotion m)
        {
            if (!middleDragging) return;

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

        //content gets scaled differently to get scrolling with the scrollbars to work
        content.CustomMinimumSize = newZoom * 3000;

        tileMapLayer.Scale = newZoom;
        grid.Scale = newZoom;
    }
}
