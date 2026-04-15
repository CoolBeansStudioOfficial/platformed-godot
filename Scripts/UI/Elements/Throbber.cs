using Godot;
using System;

public partial class Throbber : Panel
{
	[Export] double rotateSpeed;

	float rotation = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PivotOffsetRatio = new(0.5f, 0.5f);
		RotationDegrees += (float)(rotateSpeed * delta);
	}
}
