using Godot;
using System;

public partial class LevelsMenu : Control
{
	[Export] LevelsList list;
    [Export] LineEdit searchBox;

    public override void _Ready()
	{
		GetExploreLevels();
	}

	async void GetExploreLevels()
	{
		list.SetLevels(await GameManager.Instance.BrowseLevelsFromAPI());
    }
}
