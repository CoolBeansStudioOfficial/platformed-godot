using Godot;
using System;

public partial class PlayerBackground : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ThemeManager.Instance.ApplyTheme(this);
	}
}
