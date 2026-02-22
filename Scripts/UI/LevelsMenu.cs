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
    [Export] Label levelDescription;

    [Export] Label ratingCount;
    [Export] Label ratingPercentagePositive;
    [Export] Label ratingPercentageNegative;
    [Export] ProgressBar ratingBar;
    [Export] Label playCount;
    [Export] Label playDNFPercentage;
    [Export] Label playFinishPercentage;
    [Export] ProgressBar playBar;


    Level selectedLevel;

    public override void _Ready()
	{
        backButton.Pressed += OnBackButtonPressed;
        playButton.Pressed += OnPlayButtonPressed;

		ViewExplore();
	}

	public async void ViewExplore()
	{
        ShowLevelsList(true);

		levelsList.SetLevels(await GameManager.Instance.BrowseLevelsFromAPI());
    }

    public void ViewLevel(Level level)
    {
        selectedLevel = level;

        levelName.Text = level.Name;
        levelCreator.Text = level.Owner.ToString();
        levelDescription.Text = level.Description;

        ratingCount.Text = $"{level.Approvals + level.Disapprovals} ratings";
        ratingPercentageNegative.Text = $"{100 - level.ApprovalPercentage}%";
        ratingPercentagePositive.Text = $"{level.ApprovalPercentage}%";
        ratingBar.Value = level.ApprovalPercentage;

        playCount.Text = $"{level.TotalPlays.ToString()} plays";
        if (level.FinishedPlays == 0)
        {
            playDNFPercentage.Text = "100%";
            playFinishPercentage.Text = "0%";
        }
        else
        {
            //subtract finished plays from total plays
            playDNFPercentage.Text = $"{100 - Mathf.Clamp(Convert.ToInt32(((float)level.FinishedPlays / (float)level.TotalPlays) * 100f), 0, 100)}%";

            playFinishPercentage.Text = $"{Mathf.Clamp(Convert.ToInt32(((float)level.FinishedPlays / (float)level.TotalPlays) * 100f), 0, 100).ToString()}%";
            playBar.Value = Mathf.Clamp(Convert.ToInt32(((float)level.FinishedPlays / (float)level.TotalPlays) * 100f), 0, 100);
        }

        

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
