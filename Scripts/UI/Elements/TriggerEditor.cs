using Godot;
using System;
using System.Collections.Generic;

public partial class TriggerEditor : Window
{
    [Export] VBoxContainer commandContainer;
    [Export] PackedScene commandScene;

    [ExportGroup("Buttons")]
    [Export] Button apply;
    [Export] Button close;
    [Export] Button addCommand;

    TriggerParams trigger;
    TriggerParams referenceTrigger;
    List<TriggerCommand> commands = [];

	public override void _Ready()
	{
        CloseRequested += OnCloseRequested;

        apply.Pressed += ApplyPressed;
        addCommand.Pressed += AddCommandPressed;
        close.Pressed += OnCloseRequested;
	}

    void ApplyPressed()
    {
        //change reference trigger values to edited trigger values
        referenceTrigger.Execute = trigger.Execute;
        referenceTrigger.X = trigger.X;
        referenceTrigger.Y = trigger.Y;

        OnCloseRequested();
    }

    void AddCommandPressed()
    {
        AddCommand(new());
    }

    void OnCloseRequested()
    {
        Hide();
    }

    public void SetTrigger(TriggerParams editorTrigger)
    {
        //clear old commands
        if (commands.Count > 0) foreach (var command in commands)
        {
            command.QueueFree();
        }
        commands.Clear();

        //save reference to trigger saved in editor
        referenceTrigger = editorTrigger;

        //create copy of trigger
        trigger = new()
        {
            Execute = [],
            X = editorTrigger.X,
            Y = editorTrigger.Y,
        };

        foreach (var execute in referenceTrigger.Execute)
        {
            AddCommand(execute);
        }
    }
    
    void AddCommand(Execute execute)
    {
        //create new execute
        Execute copy = new()
        {
            Type = execute.Type,
            X = execute.X,
            Y = execute.Y,
            Block = execute.Block,
            Rotation = execute.Rotation,
            Time = execute.Time
        };
        trigger.Execute.Add(copy);

        //create ui
        var command = commandScene.Instantiate() as TriggerCommand;
        command.SetCommand(copy);
        command.triggerEditor = this;
        commandContainer.AddChild(command);
        commands.Add(command);
    }

    public void MoveCommandForward(TriggerCommand command)
    {
        int index = commands.IndexOf(command);
        if (index == commands.Count - 1) return;

        commands.Remove(command);
        commands.Insert(index + 1, command);
        commandContainer.MoveChild(command, command.GetIndex() + 1);

        trigger.Execute.Remove(command.command);
        trigger.Execute.Insert(index + 1, command.command);

    }

    public void MoveCommandBack(TriggerCommand command)
    {
        int index = commands.IndexOf(command);
        if (index == 0) return;

        commands.Remove(command);
        commands.Insert(index - 1, command);
        commandContainer.MoveChild(command, command.GetIndex() - 1);

        trigger.Execute.Remove(command.command);
        trigger.Execute.Insert(index - 1, command.command);
    }

    public void RemoveCommand(TriggerCommand command)
    {
        trigger.Execute.Remove(command.command);
        commands.Remove(command);
        command.QueueFree();
    }
}
