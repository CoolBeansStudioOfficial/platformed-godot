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
        delete.Pressed += DeletePressed;
        commandType.ItemSelected += CommandTypeSelected;
        positionX.ValueChanged += PositionXChanged;
        positionY.ValueChanged += PositionYChanged;
        rotation.ItemSelected += RotationChanged;
        tileID.ItemSelected += TileIDChanged;
        milliseconds.ValueChanged += MillisecondsChanged;


        //get theme
        ThemeManager.Instance.ApplyTheme(this, true);
    }

    void DeletePressed()
    {
        triggerEditor.RemoveCommand(this);
    }

    void CommandTypeSelected(long index)
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

    void PositionXChanged(double position)
    {
        command.X = (int)position;
    }

    void PositionYChanged(double position)
    {
        command.Y = (int)position;
    }

    void RotationChanged(long rotation)
    {
        command.Rotation = (int)rotation;
    }

    void TileIDChanged(long id)
    {
        command.Block = (int)id;
    }

    void MillisecondsChanged(double ms)
    {
        command.Time = Convert.ToString((int)ms);
    }

    public void SetCommand(Execute execute)
    {
        command = execute;

        //assign default values if null
        command.X ??= 0;
        command.Y ??= 0;
        command.Block ??= 0;
        if (command.Time == string.Empty) command.Time = "0";

        if (command.Type == "toggleBlocks")
        {
            CommandTypeSelected(0);
        }
        else if (command.Type == "teleport")
        {
            CommandTypeSelected(1);

            positionX.Value = command.X.Value;
            positionY.Value = command.Y.Value;
        }
        else if (command.Type == "rotate")
        {
            CommandTypeSelected(2);

            positionX.Value = command.X.Value;
            positionY.Value = command.Y.Value;
        }
        else if (command.Type == "updateBlock" || command.Type == "change")
        {
            CommandTypeSelected(3);

            positionX.Value = command.X.Value;
            positionY.Value = command.Y.Value;
            tileID.Select(command.Block.Value);
        }
        else if (command.Type == "delay")
        {
            CommandTypeSelected(4);

            milliseconds.Value = Convert.ToInt32(command.Time);
        }
    }
}
