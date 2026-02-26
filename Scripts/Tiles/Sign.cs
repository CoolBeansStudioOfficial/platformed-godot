using Godot;
using System;

public partial class Sign : Tile
{
	[Export] Texture2D rightTexture;
	[Export] Texture2D leftTexture;
    [Export] Texture2D upTexture;
	[Export] Texture2D downTexture;

    public override void OnTileCreated()
    {
        RotationDegrees = 0;

        if (info.id == TileId.SignRight) Texture = rightTexture;
        else if (info.id == TileId.SignLeft) Texture = leftTexture;
        else if (info.id == TileId.SignUp) Texture = upTexture;
        else if (info.id == TileId.SignDown) Texture = downTexture;

        SetSignRotation();
    }

	public override void OnBodyEntered(Node2D body)
    {
        //just be a chill guy
    }

    void SetSignRotation()
    {
        if (info.rotation == TileRotation.Up)
        {
            RegionRect = new()
            {
                Position = new Vector2(0, 0),
                Size = new(16, 16)
            };
        }
        else if (info.rotation == TileRotation.Left)
        {
            RegionRect = new()
            {
                Position = new Vector2(16, 0),
                Size = new(16, 16)
            };
        }
        else if (info.rotation == TileRotation.Down)
        {
            RegionRect = new()
            {
                Position = new Vector2(32, 0),
                Size = new(16, 16)
            };
        }
        else
        {
            RegionRect = new()
            {
                Position = new Vector2(48, 0),
                Size = new(16, 16)
            };
        }
    }

}
