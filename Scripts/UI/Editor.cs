using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] TilesViewport viewport;
    [Export] Control rowsContainer;
    [Export] PackedScene rowScene;
    [Export] PackedScene tileScene;

    List<EditorRow> rows = [];
	List<EditorTile> tiles;

    public override void _Ready()
    {
        for (int i = 0; i < 10; i++)
        {
            //create new row
            EditorRow row = rowScene.Instantiate() as EditorRow;
            rowsContainer.AddChild(row);
            rows.Add(row);

            //add tiles to row
            for (int j = 0; j < 10; j++)
            {
                EditorTile tile = tileScene.Instantiate() as EditorTile;
                row.AddChild(tile);
            }
        }
    }

	public void ImportLevel(Level level)
	{

	}
}
