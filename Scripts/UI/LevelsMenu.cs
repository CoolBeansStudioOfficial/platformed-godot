using Godot;
using System;

public partial class LevelsMenu : Control
{
    [Export] Control searchBar;
    [Export] LevelsList levelsList;
    [Export] Control levelView;

    [Export] LineEdit searchBox;

    [Export] Button backButton;
    [Export] Button playButton;
    [Export] Label levelName;
    [Export] Label levelCreator;

    Level selectedLevel;

    public override void _Ready()
	{
        backButton.Pressed += OnBackButtonPressed;
        playButton.Pressed += OnPlayButtonPressed;

		ViewExplore();
	}

	public async void ViewExplore()
	{
		levelsList.SetLevels(await GameManager.Instance.BrowseLevelsFromAPI());

        ShowLevelsList(true);
    }

    public void ViewLevel(Level level)
    {
        selectedLevel = level;

        levelName.Text = level.Name;
        levelCreator.Text = level.Owner.ToString();

        ShowLevelsList(false);
    }

    void OnPlayButtonPressed()
    {
        if (selectedLevel is not null)
        {
            GameManager.Instance.PlayLevel(selectedLevel);
        }

    }

    void OnBackButtonPressed()
    {
        ShowLevelsList(true);
    }

    void ShowLevelsList(bool doShow)
    {
        searchBar.Visible = doShow;
        levelsList.Visible = doShow;
        levelView.Visible = !doShow;
    }
}
