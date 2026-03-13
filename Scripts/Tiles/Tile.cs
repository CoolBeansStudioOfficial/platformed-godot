using Godot;
using System;

public struct TileInfo
{
	public Vector2I position;
	public TileRotation rotation;
    public TileId id;

    public bool tileAbove;
    public bool tileBelow;
    public bool tileLeft;
    public bool tileRight;

    public TriggerParams triggerParams;

    public int GetAdjacency()
    {
        if (!tileAbove && !tileBelow && !tileLeft && !tileRight) return 0;

        if (tileAbove && !tileBelow && !tileLeft && !tileRight) return 1;

        if (!tileAbove && !tileBelow && !tileLeft && tileRight) return 2;

        if (tileAbove && !tileBelow && !tileLeft && tileRight) return 3;

        if (!tileAbove && tileBelow && !tileLeft && !tileRight) return 4;

        if (tileAbove && tileBelow && !tileLeft && !tileRight) return 5;

        if (!tileAbove && tileBelow && !tileLeft && tileRight) return 6;

        if (tileAbove && tileBelow && !tileLeft && tileRight) return 7;

        if (!tileAbove && !tileBelow && tileLeft && !tileRight) return 8;

        if (tileAbove && !tileBelow && tileLeft && !tileRight) return 9;

        if (!tileAbove && !tileBelow && tileLeft && tileRight) return 10;

        if (tileAbove && !tileBelow && tileLeft && tileRight) return 11;

        if (!tileAbove && tileBelow && tileLeft && !tileRight) return 12;

        if (tileAbove && tileBelow && tileLeft && !tileRight) return 13;

        if (!tileAbove && tileBelow && tileLeft && tileRight) return 14;

        if (tileAbove && tileBelow && tileLeft && tileRight) return 15;

        return 0;
    }
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
    InvisibleTrigger,
    TriggerThree,
    SignRight,
    SignLeft,
    SignUp,
    SignDown,
    HalfSpikesRight,
    HalfSpikesLeft
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
    [Export] public CollisionShape2D collisionShape;

    public TileInfo info;

    public abstract void OnTileCreated();

    public abstract void OnBodyEntered(Node2D body);

    public void UpdateTile(TileInfo newInfo)
    {
        //set this tile's info
        info = newInfo;

        SetRotation(info);

        OnTileCreated();

        //subscribe to onbodyentered if applicable
        if (collisionObject is Area2D area) area.BodyEntered += OnBodyEntered;
    }

    public virtual void SetRotation(TileInfo rotationInfo)
    {
        //set rotation from info
        //this works for 90% of tiles. if you need custom rotation (i.e. signs), it can be implemented in OnTileCreated.
        if (rotationInfo.rotation == TileRotation.Up) RotationDegrees = 0;
        else if (rotationInfo.rotation == TileRotation.Left) RotationDegrees = -90;
        else if (rotationInfo.rotation == TileRotation.Down) RotationDegrees = 180;
        else RotationDegrees = 90;
    }

    
}
