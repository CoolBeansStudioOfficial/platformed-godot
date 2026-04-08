using Godot;
using System;
using System.Collections.Generic;

public partial class TriggerEditor : Window
{
    [Export] VBoxContainer commandContainer;
    [Export] PackedScene commandScene;

    TriggerParams trigger;
    List<TriggerCommand> commands = [];

	public override void _Ready()
	{
        CloseRequested += OnCloseRequested;
	}

    private void OnCloseRequested()
    {
        Hide();
    }

    public void SetTrigger(TriggerParams newTrigger)
    {
        //clear old commands
        foreach (var command in commands)
        {
            command.QueueFree();
        }
        commands.Clear();

        trigger = newTrigger;

        foreach (var execute in trigger.Execute)
        {
            var command = commandScene.Instantiate() as TriggerCommand;
            command.SetCommand(execute);
            commandContainer.AddChild(command);
            commands.Add(command);
        }
    }
}
