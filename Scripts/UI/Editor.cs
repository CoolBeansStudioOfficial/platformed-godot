using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] public Vector2I gridSize;
    [Export] TilesViewport viewport;
    [Export] TileMapLayer tileMapLayer;

    List<List<TileInfo>> tiles = [];

    public override void _Ready()
    {
        viewport.viewportSize = gridSize * 16;

        for (int i = 0; i < gridSize.Y; i++)
        {

            //add tiles to row
            for (int j = 0; j < gridSize.X; j++)
            {
            }
        }
    }

    public override void _Process(double delta)
    {
        tileMapLayer.Clear();

        var mouse = tileMapLayer.LocalToMap(tileMapLayer.GetLocalMousePosition());
        tileMapLayer.SetCell(mouse, 1, Vector2I.Zero);
    }

	public void ImportLevel(Level level)
	{

	}
}
