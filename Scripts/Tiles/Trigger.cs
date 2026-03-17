using Godot;
using System;

public partial class Trigger : Tile
{
    TriggerParams triggerParams;

    public override void OnBodyEntered(Node2D body)
    {
        if (triggerParams is null)
        {
            triggerParams = LevelManager.Instance.GetTriggerParams(Position);

            if (triggerParams is null) GD.Print("no params found for trigger");
        }

        if (body is PlayerMovement player)
        {
            if (triggerParams is not null) RunTrigger();
        }
    }

    void RunTrigger()
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
                LevelManager.Instance.SetTile(new()
                {
                    position = new(command.X.Value, command.Y.Value),
                    rotation = TileRotation.Up,
                    id = (TileId)command.Block.Value,
                });
            }
            else if (command.Type == "rotate")
            {
                throw new NotImplementedException();
            }
        }
    }
}
