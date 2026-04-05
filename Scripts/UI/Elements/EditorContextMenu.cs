using Godot;
using System;

public partial class EditorContextMenu : Panel
{
    [Export] Button fill;
    [Export] Button copy;
    [Export] Button cut;
    [Export] Button paste;
    [Export] Button rotateLeft;
    [Export] Button rotateRight;

    public Option selectedOption;

    public event Action OptionPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        fill.Pressed += () => OnOptionPressed(Option.Fill);
        copy.Pressed += () => OnOptionPressed(Option.Copy);
        cut.Pressed += () => OnOptionPressed(Option.Cut);
        paste.Pressed += () => OnOptionPressed(Option.Paste);
        rotateLeft.Pressed += () => OnOptionPressed(Option.RotateLeft);
        rotateRight.Pressed += () => OnOptionPressed(Option.RotateRight);
    }

    public enum Option
    {
        Fill,
        Copy,
        Cut,
        Paste,
        RotateLeft,
        RotateRight,
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void OnOptionPressed(Option option)
    {
        selectedOption = option;

        Action handler = OptionPressed;
        if (handler is not null) handler();
    }
}
