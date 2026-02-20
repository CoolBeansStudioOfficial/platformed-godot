using Godot;
using System;

public struct TileInfo
{
	public Vector2 position;
	public TileRotation rotation;
    public TileId id;

    public bool tileAbove;
    public bool tileBelow;
    public bool tileLeft;
    public bool tileRight;
}

public enum TileId
{
    Air,
    Ground,
    Spikes,
    SpikesDouble,
    Checkpoint,
    Spawn,
    End,
    Cactus,
    Crate,
    BouncePad,
    Coin,
    DissipationBlock,
    Enemy

}

public enum TileRotation
{
    Up,
    Left,
    Down,
    Right
}

public partial class Tile : Sprite2D
{
    [Export] CollisionShape2D staticBodyShape;
    [Export] CollisionShape2D areaShape;
    [Export] Area2D area;

    [Export] Shape2D[] hitboxShapes;


    [Export] Texture2D[] textures;

	public TileInfo info;

    private void OnBodyEntered(Node2D body)
    {
        //do different things based on what type of tile this
        if (body is PlayerMovement player)
        {
            if (info.id == TileId.Spikes || info.id == TileId.SpikesDouble || info.id == TileId.Cactus)
            {
                LevelManager.Instance.KillPlayer();
            }

            if (info.id == TileId.Checkpoint)
            {
                LevelManager.Instance.spawnPoint = Position;
            }

            if (info.id == TileId.End)
            {
                //show victory screen
            }

            if (info.id == TileId.BouncePad)
            {
                player.Velocity = new(player.Velocity.X, -800);
            }

            if (info.id == TileId.Coin)
            {
                //collect coin

                //delete self
                QueueFree();
            }
        }
    }

    public void UpdateTile(TileInfo newInfo)
    {
        //set this tile's info
        info = newInfo;

        //set rotation from info
        if (info.rotation == TileRotation.Up) RotationDegrees = 0;
        else if (info.rotation == TileRotation.Left) RotationDegrees = -90;
        else if (info.rotation == TileRotation.Down) RotationDegrees = 180;
        else RotationDegrees = 90;

        //set tile texture to match tile info
        if (textures[(int)info.id] is not null)
        {
            Texture = textures[(int)info.id];
        }

        if (info.id == TileId.Ground) SetFacingTexture();

        //enable/disable collisions based on tile type
        UpdateCollisions();
    }

    void UpdateCollisions()
    {
        //tile should be a trigger
        if (info.id == TileId.Spikes || info.id == TileId.SpikesDouble || info.id == TileId.Checkpoint || info.id == TileId.End || info.id == TileId.Cactus || info.id == TileId.BouncePad || info.id == TileId.Coin)
        {
            SetTrigger(true);
        }
        //tile should be solid block
        else SetTrigger(false);

        //update collision shape
        if (hitboxShapes[(int)info.id] is not null)
        {
            staticBodyShape.Shape = hitboxShapes[(int)info.id];
            areaShape.Shape = hitboxShapes[(int)info.id];
        }
    }

    //if set trigger is true, tile becomes a trigger. otherwise, it becomes a solid block
    void SetTrigger(bool isTrigger)
    {
        if (isTrigger)
        {
            staticBodyShape.Disabled = true;
            areaShape.Disabled = false;
            area.BodyEntered += OnBodyEntered;
        }
        else
        {
            staticBodyShape.Disabled = false;
            areaShape.Disabled = true;
        }
    }

    void SetFacingTexture()
    {
        if (!info.tileAbove && !info.tileBelow && !info.tileLeft && !info.tileRight) return;

        if (info.tileAbove && !info.tileBelow && !info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(16, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && !info.tileBelow && !info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(32, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && !info.tileBelow && !info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(48, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && info.tileBelow && !info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(64, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && info.tileBelow && !info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(80, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && info.tileBelow && !info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(96, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && info.tileBelow && !info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(112, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && !info.tileBelow && info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(128, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && !info.tileBelow && info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(144, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && !info.tileBelow && info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(160, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && !info.tileBelow && info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(176, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && info.tileBelow && info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(192, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && info.tileBelow && info.tileLeft && !info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(208, 0),
                Size = new(16, 16)
            };
        }

        if (!info.tileAbove && info.tileBelow && info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(224, 0),
                Size = new(16, 16)
            };
        }

        if (info.tileAbove && info.tileBelow && info.tileLeft && info.tileRight)
        {
            RegionRect = new()
            {
                Position = new Vector2(240, 0),
                Size = new(16, 16)
            };
        }
    }
}
