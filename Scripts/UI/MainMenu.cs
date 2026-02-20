using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] LineEdit levelIdInput;
    [Export] Button playButton;
    
	public override void _Ready()
	{
		playButton.Pressed += OnButtonPressed;
	}

	async void OnButtonPressed()
	{
		bool success = await GameManager.Instance.PlayLevel(Convert.ToInt32(levelIdInput.Text));

		if (success)
		{
			levelIdInput.Editable = false;
			playButton.Disabled = true;
			Visible = false;
		}
	}
}
