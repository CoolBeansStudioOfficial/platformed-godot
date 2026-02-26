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
        //just be a chill guy
    }

    public override void OnBodyEntered(Node2D body)
    {
        //just be a chill guy
    }

    public override void SetRotation(TileInfo rotationInfo)
    {
        RotationDegrees = 0;

        if (rotationInfo.id == TileId.SignRight) Texture = rightTexture;
        else if (rotationInfo.id == TileId.SignLeft) Texture = leftTexture;
        else if (rotationInfo.id == TileId.SignUp) Texture = upTexture;
        else if (rotationInfo.id == TileId.SignDown) Texture = downTexture;

        if (rotationInfo.rotation == TileRotation.Up)
        {
            RegionRect = new()
            {
                Position = new Vector2(0, 0),
                Size = new(16, 16)
            };
        }
        else if (rotationInfo.rotation == TileRotation.Left)
        {
            RegionRect = new()
            {
                Position = new Vector2(16, 0),
                Size = new(16, 16)
            };
        }
        else if (rotationInfo.rotation == TileRotation.Down)
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
