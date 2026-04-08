using Godot;
using System;
using System.Threading.Tasks;

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

    async Task RunTrigger()
    {
        foreach (var command in triggerParams.Execute)
        {
            GD.Print(command.Type);
            if (command.Type == "toggleBlocks")
            {
                LevelManager.Instance.redBlocksActive = !LevelManager.Instance.redBlocksActive;
            }
            else if (command.Type == "teleport")
            {
                LevelManager.Instance.TeleportPlayer(new(command.X.Value, command.Y.Value));
            }
            else if (command.Type == "updateBlock" || command.Type == "change")
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
                LevelManager.Instance.RotateTile(new(command.X.Value, command.Y.Value), (TileRotation)command.Rotation.Value);
            }
            else if (command.Type == "fill")
            {
                for (int y = command.StartY.Value; y <= command.EndY.Value; y++)
                {
                    for (int x = command.StartX.Value; x <= command.EndX.Value; x++)
                    {
                        LevelManager.Instance.SetTile(new()
                        {
                            position = new(x, y),
                            rotation = TileRotation.Up,
                            id = (TileId)command.Block.Value,
                        });
                    }
                }
                
            }
            else if (command.Type == "delay")
            {
                await Task.Delay(Convert.ToInt32(command.Time));
            }
        }
    }
}
