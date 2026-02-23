using Godot;
using System;

public partial class ToggleBlock : Tile
{
    [Export] bool isRedBlock;

    public override void _Process(double delta)
    {
        if (isRedBlock)
        {
            Visible = LevelManager.Instance.redBlocksActive;
            collisionShape.Disabled = !LevelManager.Instance.redBlocksActive;
        }
        else
        {
            Visible = !LevelManager.Instance.redBlocksActive;
            collisionShape.Disabled = LevelManager.Instance.redBlocksActive;
        }
    }

    public override void OnBodyEntered(Node2D body)
    {
        //just be a chill guy
    }
}
