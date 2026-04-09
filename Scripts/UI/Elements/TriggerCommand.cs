using Godot;
using System;

public partial class TriggerCommand : Control
{
	[ExportGroup("Show & Hide Controls")]
    [Export] Control position;
    [Export] Control endPosition;
    [Export] Control delay;

    [ExportGroup("Inputs")]
    [Export] OptionButton commandType;
    [Export] SpinBox positionX;
    [Export] SpinBox positionY;
    [Export] SpinBox endPositionX;
    [Export] SpinBox endPositionY;
    [Export] OptionButton rotation;
    [Export] OptionButton tileID;
    [Export] SpinBox milliseconds;
    [Export] Button delete;
    [Export] Button moveUp;
    [Export] Button moveDown;

    public Execute command = new();
    public TriggerEditor triggerEditor;

    public override void _Ready()
	{
        delete.Pressed += DeletePressed;
        moveUp.Pressed += MoveUp;
        moveDown.Pressed += MoveDown;
        commandType.ItemSelected += CommandTypeSelected;
        positionX.ValueChanged += PositionXChanged;
        positionY.ValueChanged += PositionYChanged;
        endPositionX.ValueChanged += EndPositionXChanged;
        endPositionY.ValueChanged += EndPositionYChanged;
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

    void MoveUp()
    {
        triggerEditor.MoveCommandBack(this);
    }

    void MoveDown()
    {
        triggerEditor.MoveCommandForward(this);
    }

    void CommandTypeSelected(long index)
    {
        if (index == 0)
        {
            command.Type = "toggleBlocks";
            commandType.Select(0);

            position.Hide();
            endPosition.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 1)
        {
            command.Type = "teleport";
            commandType.Select(1);

            position.Show();
            endPosition.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 2)
        {
            command.Type = "rotate";
            commandType.Select(2);

            position.Show();
            endPosition.Hide();
            rotation.Show();
            tileID.Hide();
            delay.Hide();
        }
        else if (index == 3)
        {
            command.Type = "change";
            commandType.Select(3);

            position.Show();
            endPosition.Hide();
            rotation.Hide();
            tileID.Show();
            delay.Hide();
        }
        else if (index == 4)
        {
            command.Type = "delay";
            commandType.Select(4);

            position.Hide();
            endPosition.Hide();
            rotation.Hide();
            tileID.Hide();
            delay.Show();
        }
        else if (index == 5)
        {
            command.Type = "fill";
            commandType.Select(5);

            position.Show();
            endPosition.Show();
            rotation.Hide();
            tileID.Show();
            delay.Hide();
        }
    }

    void PositionXChanged(double position)
    {
        command.X = (int)position;
        command.StartX = (int)position;
    }

    void PositionYChanged(double position)
    {
        command.Y = (int)position;
        command.StartY = (int)position;
    }

    void EndPositionXChanged(double position)
    {
        command.EndX = (int)position;
    }

    void EndPositionYChanged(double position)
    {
        command.EndY = (int)position;
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
        command.Time = Convert.ToString(ms);
    }

    public void SetCommand(Execute execute)
    {
        command = execute;
        GD.Print($"start: ({command.StartX}, {command.StartY}) end: ({command.EndX}, {command.EndY})");

        //assign default values if null
        if (command.Type == string.Empty || command.Type is null) command.Type = "toggleBlocks";
        command.X ??= 0;
        command.StartX ??= 0;
        command.EndX ??= 0;
        command.Y ??= 0;
        command.StartY ??= 0;
        command.EndY ??= 0;
        command.Rotation ??= 0;
        command.Block ??= 0;

        if (command.Time == string.Empty || command.Time is null) command.Time = "0";

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

            milliseconds.Value = Convert.ToDouble(command.Time);
        }
        else if (command.Type == "fill")
        {
            CommandTypeSelected(5);

            positionX.Value = command.StartX.Value;
            endPositionX.Value = command.EndX.Value;
            positionY.Value = command.StartY.Value;
            endPositionY.Value = command.EndY.Value;
            tileID.Select(command.Block.Value);
        }
    }
}
