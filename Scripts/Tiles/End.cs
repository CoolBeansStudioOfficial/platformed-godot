using Godot;
using System;

public partial class End : Tile
{
    [Export] AudioStream victorySound;

    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            AudioManager.Instance.PlayStream(victorySound);
            UIManager.Instance.mainMenu.Visible = true;
            LevelManager.Instance.DestroyLevel();
        }
    }
}
