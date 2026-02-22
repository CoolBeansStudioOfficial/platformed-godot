using Godot;
using System;

public partial class UIManager : Node
{
	[Export] public Control levelsMenu;
    [Export] public Control pauseMenu;

    //singleton pringleton
    public static UIManager Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			//only show pause menu if in game
			if (levelsMenu.Visible == false)
			{
				pauseMenu.Visible = !pauseMenu.Visible;
			}
		}

		if (levelsMenu.Visible == true) pauseMenu.Visible = false;
	}
}
