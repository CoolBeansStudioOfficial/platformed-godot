using Godot;
using System;

public partial class TriggerCommand : Control
{
	[ExportGroup("Show & Hide Controls")]
    [Export] Control position;
    [Export] Control delay;

    [ExportGroup("Inputs")]
    [Export] OptionButton commandType;
    [Export] SpinBox positionX;
    [Export] SpinBox positionY;
    [Export] OptionButton rotation;
    [Export] OptionButton tileID;
    [Export] SpinBox milliseconds;
    [Export] Button delete;

    public Execute command = new();

    public override void _Ready()
	{
        commandType.ItemSelected += CommandTypeSelected;
        delete.Pressed += DeletePressed;
	}

    private void DeletePressed()
    {
        throw new NotImplementedException();
    }

    private void CommandTypeSelected(long index)
    {
        if (index == 0)
        {
            command.Type = "toggleBlocks";

            position.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 1)
        {
            command.Type = "teleport";

            position.Show();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 2)
        {
            command.Type = "rotate";

            position.Show();
            rotation.Show();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 3)
        {
            command.Type = "change";

            position.Show();
            rotation.Hide();
            tileID.Show();
            delay.Hide();
        }
        else if (index == 4)
        {
            command.Type = "delay";

            position.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Show();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
