using Godot;
using System;

public partial class End : Tile
{
    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement player)
        {
            UIManager.Instance.mainMenu.Visible = true;
            LevelManager.Instance.DestroyLevel();
        }
    }
}
