using Godot;
using System;

public partial class EraserButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Toggled += OnToggled;
	}

    private void OnToggled(bool toggledOn)
    {
        UIManager.Instance.editor.SelectEraser(toggledOn);
    }

    public override void _Process(double delta)
    {
        ButtonPressed = UIManager.Instance.editor.eraserSelected;
    }
}
