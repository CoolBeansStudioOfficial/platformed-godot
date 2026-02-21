using Godot;
using System;

public partial class UIManager : Node
{
	[Export] public Control mainMenu;

	//singleton pringleton
	public static UIManager Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
