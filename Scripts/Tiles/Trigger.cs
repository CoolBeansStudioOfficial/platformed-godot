using Godot;
using System;

public partial class Trigger : Tile
{
    public override void OnTileCreated()
    {
        //chill guy
    }

    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            if (info.triggerParams is not null) RunTrigger(info.triggerParams);
        }
    }


    void RunTrigger(TriggerParams triggerParams)
    {
        foreach (var command in triggerParams.Execute)
        {
            if (command.Type == "toggleBlocks")
            {
                LevelManager.Instance.redBlocksActive = !LevelManager.Instance.redBlocksActive;
            }
            else if (command.Type == "teleport")
            {
                LevelManager.Instance.TeleportPlayer(new(command.X.Value, command.Y.Value));
            }
            else if (command.Type == "updateBlock")
            {
                //spawn block deferred
                LevelManager.Instance.SpawnBlock(new()
                {
                    position = new(command.X.Value, command.Y.Value),
                    rotation = TileRotation.Up,
                    id = (TileId)command.Block.Value,
                }, 
                TileRotation.Up, true);
            }
        }
    }
}
