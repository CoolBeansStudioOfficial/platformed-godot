using Godot;
using System;

public partial class Coin : Tile
{
    [Export] AudioStream coinSound;


    bool collected = false;
    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player && !collected)
        {
            //collect coin

            //play collection sound
            AudioManager.Instance.PlayStream(coinSound);

            //hide self
            Visible = false;

            collected = true;
        }
    }
}
