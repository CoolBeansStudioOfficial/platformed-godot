using Godot;
using System;

public partial class EditorCategoryToggle : Button
{
	[Export] VBoxContainer category;
	public override void _Ready()
	{
		category.Visible = ButtonPressed;
		Toggled += OnToggled;
	}

    void OnToggled(bool toggledOn)
    {
        category.Visible = toggledOn;
    }
}
