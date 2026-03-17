using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] public Vector2I gridSize;
    [Export] TilesViewport viewport;
    [Export] TileMapLayer tileMap;
    [Export] TileMapLayer overlayMap;

    [ExportGroup("UI")]
    [Export] Label nameLabel;
    [Export] Label unsavedLabel;
    [Export] Button backButton;

    Level currentLevel;
    List<List<TileInfo>> tiles = [];

    public override void _Ready()
    {
        backButton.Pressed += OnBackButtonPressed;

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
        Vector2I mouseCoords = tileMap.LocalToMap(tileMap.GetLocalMousePosition());

        //return if mouse is outside of set level size
        if (mouseCoords.X >= gridSize.X || mouseCoords.Y >= gridSize.Y) return;
        if (mouseCoords.X < 0  || mouseCoords.Y < 0) return;

        if (viewport.clicking)
        {
            tileMap.SetCell(mouseCoords, 1, Vector2I.Zero);
        }

        if (viewport.rightClicking)
        {
            tileMap.SetCell(mouseCoords);

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
                tileMap.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }

    public void ImportLevel(Level level)
	{
        currentLevel = level;
        nameLabel.Text = level.Name;

        tiles = LevelManager.Instance.CreateTilemap(level);

        gridSize = new(level.Width, level.Height);

        foreach (var row in tiles)
        {
            foreach (var tile in row)
            {
                tileMap.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }

    void OnBackButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu();
    }
}
