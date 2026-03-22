using Godot;
using System;

public partial class UIManager : Node
{
	[Export] public Editor editor;
    [Export] public Button returnToEditorButton;
    [Export] public LevelsMenu levelsMenu;
    [Export] public Control pauseMenu;
	[Export] public AcceptDialog notification;

    //singleton pringleton
    public static UIManager Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			//only show pause menu if in game
			if (!levelsMenu.Visible && !editor.Visible)
			{
				pauseMenu.Visible = !pauseMenu.Visible;
			}
		}

		if (levelsMenu.Visible) pauseMenu.Visible = false;
	}

    public void PopupNotification(string message, string title = "Notification")
    {
		notification.DialogText = message;
		notification.Title = title;
		notification.Popup();
    }
}
