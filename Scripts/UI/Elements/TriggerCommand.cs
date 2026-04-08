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
    public TriggerEditor triggerEditor;

    public override void _Ready()
	{
        commandType.ItemSelected += CommandTypeSelected;
        delete.Pressed += DeletePressed;

        //get theme
        ThemeManager.Instance.ApplyTheme(this, true);
    }

    private void DeletePressed()
    {
        throw new NotImplementedException();
    }

    public void SetCommand(Execute execute)
    {
        command = execute;

        if (command.Type == "toggleBlocks")
        {
            CommandTypeSelected(0);
        }
        if (command.Type == "teleport")
        {
            CommandTypeSelected(1);

            positionX.Value = (double)command.X;
            positionY.Value = (double)command.Y;
        }
        if (command.Type == "rotate")
        {
            CommandTypeSelected(2);

            positionX.Value = (double)command.X;
            positionY.Value = (double)command.Y;
        }
        if (command.Type == "updateBlock" || command.Type == "change")
        {
            CommandTypeSelected(3);

            positionX.Value = (double)command.X;
            positionY.Value = (double)command.Y;
            tileID.Select(command.Block.Value);
        }
        if (command.Type == "delay")
        {
            CommandTypeSelected(4);

            milliseconds.Value = Convert.ToInt32(command.Time);
        }
    }

    private void CommandTypeSelected(long index)
    {
        if (index == 0)
        {
            command.Type = "toggleBlocks";
            commandType.Select(0);

            position.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 1)
        {
            command.Type = "teleport";
            commandType.Select(1);

            position.Show();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 2)
        {
            command.Type = "rotate";
            commandType.Select(2);

            position.Show();
            rotation.Show();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 3)
        {
            command.Type = "change";
            commandType.Select(3);

            position.Show();
            rotation.Hide();
            tileID.Show();
            delay.Hide();
        }
        else if (index == 4)
        {
            command.Type = "delay";
            commandType.Select(4);

            position.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Show();
        }
    }

    public Execute GetExecute()
    {
        return new()
        {
            Type = command.Type,
            X = (int)positionX.Value,
            Y = (int)positionY.Value,
            Block = tileID.Selected,
            Rotation = rotation.Selected,
            Time = Convert.ToString((int)milliseconds.Value)
        };
    }
}
