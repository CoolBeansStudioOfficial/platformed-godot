using Godot;
using System;

public partial class Sign : Tile
{
	[Export] Texture2D rightTexture;
	[Export] Texture2D leftTexture;
    [Export] Texture2D upTexture;
	[Export] Texture2D downTexture;

    public override void _Ready()
	{
        if (info.id == TileId.SignRight) Texture = rightTexture;
        else if (info.id == TileId.SignLeft) Texture = leftTexture;
        else if (info.id == TileId.SignUp) Texture = upTexture;
        else if (info.id == TileId.SignDown) Texture = downTexture;

    }

	public override void OnBodyEntered(Node2D body)
    {
        //just be a chill guy
    }

}
