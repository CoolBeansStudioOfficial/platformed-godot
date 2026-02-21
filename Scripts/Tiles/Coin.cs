using Godot;
using System;

public partial class Coin : Tile
{
    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            //collect coin

            //play collection sound

            //hide self
            Visible = false;

        }
    }
}
