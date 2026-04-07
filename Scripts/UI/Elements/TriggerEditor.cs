using Godot;
using System;

public partial class TriggerEditor : Window
{
    [Export] VBoxContainer commandContainer;

    TriggerParams trigger;

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
        trigger = newTrigger;


    }
}
