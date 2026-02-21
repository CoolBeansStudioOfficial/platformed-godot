using Godot;
using System;

public partial class Checkpoint : Tile
{
    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player) LevelManager.Instance.spawnPoint = Position;
    }
}
