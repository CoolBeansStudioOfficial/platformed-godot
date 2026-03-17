using Godot;
using System;

public partial class EditorSideBar : HBoxContainer
{
	[Export] Panel sideBarBlocks;
	[Export] Button[] buttons;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		bool showBlocks = false;

		foreach (var button in buttons)
		{
			if (button.ButtonPressed) showBlocks = true;
		}

		sideBarBlocks.Visible = showBlocks;
	}
}
