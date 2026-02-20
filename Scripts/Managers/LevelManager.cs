using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class LevelManager : Node
{
    [Export] PackedScene playerScene;
    [Export] PackedScene tileScene;

    Level currentLevel;
    List<Tile> tiles = [];

    Node2D player;
    public Vector2 spawnPoint;

    //singleton pringleton
    public static LevelManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    
    public void SpawnPlayer()
    {
        player = (Node2D)playerScene.Instantiate();
        player.Position = spawnPoint;
        AddChild(player);
    }

    public void KillPlayer()
    {
        //play death sound effect

        player.Position = spawnPoint;

        //reset any enemies or other dynamic entities
    }

    public void GenerateLevel(Level level)
    {
        currentLevel = level;
        tiles.Clear();

        //set player spawn point
        spawnPoint = new(currentLevel.Data.Spawn.X * 16, currentLevel.Data.Spawn.Y * 16);

        //get tilemap from compressed level data
        var tilemap = CreateTilemap(DecodeRLE(currentLevel.Data.Layers[0].Data, currentLevel.Width));

        //create actual blocks from tilemap

        int rowCount = 0;
        foreach (var row in tilemap)
        {
            int i = 0;
            foreach (var tile in row)
            {
                if (tile.id == TileId.Air || tile.id == TileId.Spawn)
                {
                    //empty tile
                }
                else
                {
                    SpawnBlock(tile);
                }

                i++;
            }

            rowCount++;
        }
    }

    void SpawnBlock(TileInfo info)
    {
        Node2D block = (Node2D)tileScene.Instantiate();
        AddChild(block);
        var tile = block as Tile;
        tiles.Add(tile);
        tile.UpdateTile(info);
        block.Position = tile.info.position * 16;
    }

    //takes list of rows of tiles and returns same list but with info about each tile
    List<List<TileInfo>> CreateTilemap(List<List<int>> tiles)
    {
        List<List<TileInfo>> tilemap = [];

        int y = 0;
        foreach (var row in tiles)
        {
            List<TileInfo> currentRow = [];

            int x = 0;
            foreach (var tile in row)
            {
                //check adjacencies
                bool above = false;
                bool below = false;
                bool left = false;
                bool right = false;

                //check above, skip check if this is top row
                if (y != 0) if (tiles[y - 1][x] == tile) above = true;

                //check below, skip check if this is bottom row
                if (y != tiles.Count - 1) if (tiles[y + 1][x] == tile) below = true;

                //check left, skip if this tile is leftmost tile
                if (x != 0) if (tiles[y][x - 1] == tile) left = true;

                //check right, skip if this tile is rightmost tile
                if (x != tiles[y].Count - 1) if (tiles[y][x + 1] == tile) right = true;


                //construct tile info
                TileInfo info = new()
                {
                    position = new(x, y),
                    id = (TileId)tile,

                    tileAbove = above,
                    tileBelow = below,
                    tileLeft = left,
                    tileRight = right,
                };

                currentRow.Add(info);

                x++;
            }

            //add new row to tilemap
            List<TileInfo> toAdd = [];
            toAdd.AddRange(currentRow);
            tilemap.Add(toAdd);
            currentRow.Clear();

            y++;
        }

        return tilemap;
    }

    //takes encoded map data and converts it to a list of rows of tiles
    List<List<int>> DecodeRLE(List<JsonElement> rle, int width)
    {
        List<List<int>> map = [];

        List<int> currentRow = [];
        foreach (var element in rle)
        {
            //element represents multiple of the same tile
            if (element.ValueKind == JsonValueKind.Array)
            {
                var tile = element[0].GetInt32();
                var count = element[1].GetInt32();

                for (int i = 0; i < count; i++)
                {
                    currentRow.Add(tile);

                    if (currentRow.Count == width)
                    {
                        GD.Print($"adding row of length {currentRow.Count}");
                        List<int> toAdd = [];
                        toAdd.AddRange(currentRow);
                        map.Add(toAdd);
                        currentRow.Clear();
                    }
                }
            }
            //element represents single tile
            else
            {
                currentRow.Add(element.GetInt32());

                if (currentRow.Count == width)
                {
                    List<int> toAdd = [];
                    toAdd.AddRange(currentRow);
                    map.Add(toAdd);
                    currentRow.Clear();
                }
            }
        }

        return map;
    }
}
