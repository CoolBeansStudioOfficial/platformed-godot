using Godot;
using System;

public struct TileInfo
{
	public Vector2 position;
	public int rotation;
    public int tileType;

    public bool tileAbove;
    public bool tileBelow;
    public bool tileLeft;
    public bool tileRight;
}

public partial class Tile : Sprite2D
{
	public TileInfo info;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
    }

    public void UpdateTexture()
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
