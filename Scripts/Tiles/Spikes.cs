using Godot;
using System;

public partial class Spikes : Tile
{
    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player) LevelManager.Instance.KillPlayer();
    }
}
