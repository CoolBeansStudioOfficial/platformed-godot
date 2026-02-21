using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] PackedScene levelPreview;
	[Export] FlowContainer levelsContainer;
    
	public override void _Ready()
	{
		GetExploreLevels();
	}

	async void GetExploreLevels()
	{
		foreach (Level level in await GameManager.Instance.BrowseLevelsFromAPI())
		{
			if (level == null) return;
			LevelPreview preview = levelPreview.Instantiate() as LevelPreview;
			preview.SetLevel(level);
			levelsContainer.AddChild(preview);
		}
	}

	async void OnButtonPressed()
	{
        //levelIdInput.Editable = false;
        //playButton.Disabled = true;
        //Visible = false;
    }
}
