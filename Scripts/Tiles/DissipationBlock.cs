using Godot;
using System;
using System.Threading.Tasks;

public partial class DissipationBlock : Tile
{
    [Export] CollisionShape2D blockShape;

    [Export] float standTime;
    [Export] float respawnTime;

    public override void OnBodyEntered(Node2D body)
    {
        if (body is PlayerMovement) Dissipate();
    }

    public async void Dissipate()
    {
        //disable dissipation trigger
        collisionShape.SetDeferred("disabled", true);
        Modulate = Colors.PaleVioletRed;

        await Task.Delay((int)(standTime * 1000));

        //disable actual block
        Visible = false;
        blockShape.SetDeferred("disabled", true);

        await Task.Delay((int)(respawnTime * 1000));

        //re-enable block
        Modulate = Colors.White;
        Visible = true;
        collisionShape.SetDeferred("disabled", false);
        blockShape.SetDeferred("disabled", false);
    }
}
