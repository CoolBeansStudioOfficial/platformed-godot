using Godot;
using System;

public partial class BouncePad : Tile
{
    [Export] float bouncePadHeight;

    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            if (info.rotation == TileRotation.Up) player.Velocity = new(player.Velocity.X, -bouncePadHeight);
            else if (info.rotation == TileRotation.Left)
            {
                player.moveLock = 0.25f;
                player.bouncedSidewaysBySpring = true;
                player.Velocity = new(-bouncePadHeight, player.Velocity.Y);
            }
            else if (info.rotation == TileRotation.Down) player.Velocity = new(player.Velocity.X, bouncePadHeight);
            else
            {
                player.moveLock = 0.25f;
                player.bouncedSidewaysBySpring = true;
                player.Velocity = new(bouncePadHeight, player.Velocity.Y);
            }
        }
            
    }
}
