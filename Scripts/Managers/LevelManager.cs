using Godot;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;

public partial class LevelManager : Node
{
    [Export] PackedScene playerScene;
    [Export] TileMapLayer tileMapLayer;
    [Export] Godot.Collections.Array<bool> useAdjacency;
    [Export] Godot.Collections.Array<bool> sceneTile;

    [Export] AudioStream deathSound;

    Level currentLevel;

    PlayerMovement player;
    bool isPlayerSpawned = false;
    public Vector2 spawnPoint;

    public bool redBlocksActive = true;

    //singleton pringleton
    public static LevelManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (currentLevel == null || !isPlayerSpawned) return;

        //kill player if they fall off the map
        if (player.Position.Y > currentLevel.Height * 16) KillPlayer();
    }


    public void TeleportPlayer(Vector2 position)
    {
        player.Position = position * 16;
    }

    public void KillPlayer()
    {
        //play death sound effect
        AudioManager.Instance.PlayStream(deathSound);

        player.Velocity = Vector2.Zero;
        player.Position = spawnPoint;

        //reset any enemies or other dynamic entities
    }

    public void DestroyLevel()
    {
        //clear out previous tiles
        tileMapLayer.Clear();

        //destroy player
        if (isPlayerSpawned) player.QueueFree();
        isPlayerSpawned= false;

        GD.Print("level destroyed");

    }

    public void GenerateLevel(Level level)
    {
        if (level is null) return;

        //set current level
        currentLevel = level;

        //clear out previous tiles
        DestroyLevel();

        //set player spawn point
        spawnPoint = new(currentLevel.Data.Spawn.X * 16, currentLevel.Data.Spawn.Y * 16);


        //get tilemap from compressed level data
        var tilemap = CreateTilemap(DecodeRLE(currentLevel.Data.Layers[0].Data, currentLevel.Width));
        //get each tile's rotation from compressed level data
        var rotationMap = DecodeRLE(currentLevel.Data.Layers[1].Data, currentLevel.Width);

        //create actual blocks from tilemap
        int y = 0;
        foreach (var row in tilemap)
        {
            int x = 0;
            foreach (TileInfo tile in row)
            {
                if (tile.id == TileId.Air || tile.id == TileId.Spawn)
                {
                    //empty tile
                }
                else
                {
                    //spawn tile and pass in rotation from map
                    SetTile(tile, (TileRotation)rotationMap[y][x]);
                }

                x++;
            }

            y++;
        }

        //spawn player
        SpawnPlayer();
    }



    void SpawnPlayer()
    {
        player = playerScene.Instantiate() as PlayerMovement;
        player.Position = spawnPoint;
        AddChild(player);

        //configure player physics
        if (currentLevel.Data.WallJump == "off")
        {
            player.walljumpMoveLock = 0.25f;
        }
        else if (currentLevel.Data.WallJump == "up")
        {
            player.walljumpMoveLock = 0.05f;
        }

        isPlayerSpawned = true;
    }

    public void SetTile(TileInfo info, TileRotation rotation)
    {
        //get atlas coords for tile
        Vector2I atlasCoords;
        int altTile = 0;

        if (useAdjacency[(int)info.id]) atlasCoords = new(info.GetAdjacency(), 0);
        else atlasCoords = new((int)rotation, 0);

        //if scene tile, use alternate scene instead of atlas coords
        if (sceneTile[(int)info.id])
        {
            atlasCoords = Vector2I.Zero;
            altTile = (int)rotation;
        }

        //set grid cell
        tileMapLayer.SetCell(info.position, (int)info.id, atlasCoords, altTile);
    }

    public TileData GetTileFromCollision(Rid rid)
    {
        return tileMapLayer.GetCellTileData(tileMapLayer.GetCoordsForBodyRid(rid));
    }

    public TileData GetTileFromGrid(Vector2I position)
    {
        return tileMapLayer.GetCellTileData(position);
    }

    public TriggerParams GetTriggerParams(Vector2 position)
    {
        Vector2I tilePosition = tileMapLayer.LocalToMap(position);

        TriggerParams triggerParams = null;

        if (currentLevel.Data.Triggers is not null)
        {
            foreach (var trigger in currentLevel.Data.Triggers)
            {
                if (trigger.X == tilePosition.X && trigger.Y == tilePosition.Y)
                {
                    triggerParams = trigger;

                    break;
                }
            }
        }

        return triggerParams;
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
