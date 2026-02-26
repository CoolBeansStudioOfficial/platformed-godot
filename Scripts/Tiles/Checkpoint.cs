using Godot;
using System;

public partial class Checkpoint : Tile
{
    [Export] AudioStream checkpointSound;

    public override void OnTileCreated()
    {
        //just be a chill guy
    }

    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            if (LevelManager.Instance.spawnPoint != Position) AudioManager.Instance.PlayStream(checkpointSound);

            LevelManager.Instance.spawnPoint = Position;
        }

    }
}
