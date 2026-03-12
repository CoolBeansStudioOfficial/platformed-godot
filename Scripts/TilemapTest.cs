using Godot;
using System;

public partial class TilemapTest : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetCell(new Vector2I(10, 10), 0, Vector2I.Zero);
	}
}
