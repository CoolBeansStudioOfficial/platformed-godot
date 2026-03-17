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
            //add row
            tiles.Add(new());

            //add tiles to row
            for (int j = 0; j < gridSize.X; j++)
            {
                tiles[i].Add(new());
            }
        }
    }

    public override void _Process(double delta)
    {
        Vector2I mouseCoords = tileMapLayer.LocalToMap(tileMapLayer.GetLocalMousePosition());

        //return if mouse is outside of set level size
        if (mouseCoords.X >= gridSize.X || mouseCoords.Y >= gridSize.Y) return;

        if (viewport.clicking)
        {
            tileMapLayer.SetCell(mouseCoords, 1, Vector2I.Zero);
        }

        if (viewport.rightClicking)
        {
            tileMapLayer.SetCell(mouseCoords);

            tiles[mouseCoords.Y][mouseCoords.X] = new();
        }
        
        UpdateTiles();
    }

    public void UpdateTiles()
    {
        tiles = LevelManager.Instance.UpdateAdjacencies(tiles);
        
        //set cells according to stored tiles
        foreach (var row in tiles)
        {
            foreach (var tile in row)
            {
                tileMapLayer.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }

    public void ImportLevel(Level level)
	{
        tiles = LevelManager.Instance.CreateTilemap(level);

        gridSize = new(level.Width, level.Height);

        foreach (var row in tiles)
        {
            foreach (var tile in row)
            {
                tileMapLayer.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }
}
