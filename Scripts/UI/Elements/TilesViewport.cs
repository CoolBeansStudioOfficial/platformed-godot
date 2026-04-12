using Godot;
using System;
using System.Threading;

public partial class TilesViewport : ScrollContainer
{
    [Export] float minZoom;
    [Export] float maxZoom;
    [Export] float zoomSpeed;
    [Export] Control content;
    [Export] Node[] scaleItems;
    [Export] Node[] relativeItems;

    [ExportGroup("UI")]
    [Export] Slider zoomSlider;
    [Export] Button zoomOut;
    [Export] Button zoomIn;

    public Vector2 viewportSize;

    public float zoom = 1f;
    //number of inputs each action has been performed for
    public bool middleDragging = false;
    Vector2 lastMousePosition = Vector2.Zero;
    bool sliderHeld = false;

    public override void _Ready()
    {
        zoomSlider.DragStarted += OnSliderDragged;
        zoomSlider.ValueChanged += OnSliderValueChanged;
        zoomSlider.DragEnded += OnSliderRelease;

        zoomOut.Pressed += () => Zoom(-zoomSpeed * 4, new(960, 540));
        zoomIn.Pressed += () => Zoom(zoomSpeed * 4, new(960, 540));
    }


    void OnSliderDragged()
    {
        sliderHeld = true;
    }

    void OnSliderValueChanged(double value)
    {
        if (sliderHeld) Zoom((float)zoomSlider.Value - zoom, new(960, 540));
    }

    void OnSliderRelease(bool valueChanged)
    {
        sliderHeld = false;
    }

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
                }
                else
                {
                    middleDragging = false;
                    MouseDefaultCursorShape = CursorShape.Arrow;
                }
            }
            else if (mb.ButtonIndex == MouseButton.WheelUp && mb.IsCommandOrControlPressed())
            {
                Zoom(zoomSpeed, mb.Position);

                AcceptEvent();
            }
            else if (mb.ButtonIndex == MouseButton.WheelDown && mb.IsCommandOrControlPressed())
            {
                Zoom(-zoomSpeed, mb.Position);

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

    public void Zoom(float amount, Vector2 mousePosition)
    {
        float previousZoom = zoom;
        zoom += amount;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        zoomSlider.Value = zoom;

        Vector2 scroll = new(ScrollHorizontal, ScrollVertical);

        //calculate where mouse is relative to previous zoom
        Vector2 previousPosition = (scroll + mousePosition) / previousZoom;

        //calculate new scroll so the position relative to the mouse stays the same
        Vector2 newScroll = (previousPosition * zoom) - mousePosition;

        ScrollHorizontal = (int)newScroll.X;
        ScrollVertical = (int)newScroll.Y;
    }

    public override void _Process(double delta)
    {
        Vector2 newZoom = new(zoom, zoom);

        //content gets scaled differently to get scrolling with the scrollbars to work
        content.CustomMinimumSize = newZoom * 4000;

        foreach (var node in scaleItems)
        {
            if (node is Node2D n) n.Scale = newZoom;
            else if (node is Control c) c.Scale = newZoom;
        }

        foreach (var node in relativeItems)
        {
            if (node is Node2D n) n.Scale = Vector2.One / newZoom;
            else if (node is Control c) c.Scale = Vector2.One / newZoom;
        }
    }
}
