using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] public Vector2 gridSize;
    [Export] TilesViewport viewport;
    [Export] Control rowsContainer;
    [Export] PackedScene rowScene;
    [Export] PackedScene tileScene;

    List<EditorRow> rows = [];
	List<EditorTile> tiles;

    public override void _Ready()
    {
        viewport.viewportSize = gridSize * 16;

        for (int i = 0; i < gridSize.Y; i++)
        {
            //create new row
            EditorRow row = rowScene.Instantiate() as EditorRow;
            rows.Add(row);
            rowsContainer.AddChild(row);

            //add tiles to row
            for (int j = 0; j < gridSize.X; j++)
            {
                EditorTile tile = tileScene.Instantiate() as EditorTile;
                row.tiles.Add(tile);
                row.AddChild(tile);
            }
        }
    }

	public void ImportLevel(Level level)
	{

	}
}
