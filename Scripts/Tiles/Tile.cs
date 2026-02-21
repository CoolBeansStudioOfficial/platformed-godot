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
    Enemy,
    ToggleBlockBlue,
    ToggleBlockRed,
    TriggerOne,

}

public enum TileRotation
{
    Up,
    Left,
    Down,
    Right
}

public abstract partial class Tile : Sprite2D
{
    [Export] bool usesAdjacency;
    [Export] CollisionObject2D collisionObject;

	public TileInfo info;

    public abstract void OnBodyEntered(Node2D body);

    public void UpdateTile(TileInfo newInfo)
    {
        //set this tile's info
        info = newInfo;

        //set rotation from info
        if (info.rotation == TileRotation.Up) RotationDegrees = 0;
        else if (info.rotation == TileRotation.Left) RotationDegrees = -90;
        else if (info.rotation == TileRotation.Down) RotationDegrees = 180;
        else RotationDegrees = 90;

        if (usesAdjacency) SetFacingTexture();

        //subscribe to onbodyentered if applicable
        if (collisionObject is Area2D area) area.BodyEntered += OnBodyEntered;
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
