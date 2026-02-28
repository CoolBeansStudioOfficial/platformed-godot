using Godot;
using System;

public partial class LevelsMenu : Control
{
    [Export] FileDialog fileDialog;
    [Export] Button addLevelsFolderButton;

    [Export] Panel searchBar;
    [Export] RichTextLabel searchBarText;
    [Export] StyleBoxTexture exploreStyle;
    [Export] StyleBoxTexture myLevelsStyle;

    [Export] LevelsList levelsList;
    [Export] Control levelView;

    [Export] Button backButton;
    [Export] Button playButton;
    [Export] Label levelName;
    [Export] Label levelCreator;
    [Export] RichTextLabel levelDescription;

    [Export] Label ratingCount;
    [Export] Label ratingPercentagePositive;
    [Export] Label ratingPercentageNegative;
    [Export] ProgressBar ratingBar;
    [Export] Label playCount;
    [Export] Label playDNFPercentage;
    [Export] Label playFinishPercentage;
    [Export] ProgressBar playBar;

    public bool explore = true;
    Level selectedLevel;

    public override void _Ready()
	{
        fileDialog.DirSelected += OnLevelsFolderSelected;

        addLevelsFolderButton.Pressed += OnAddLevelsFolderButtonPressed;
        backButton.Pressed += OnBackButtonPressed;
        playButton.Pressed += OnPlayButtonPressed;
        Explore();
    }

    public async void Explore()
    {
        //set ui to explore menu
        searchBar.AddThemeStyleboxOverride("panel", exploreStyle);
        searchBarText.Text = "[b][font_size=29][wave freq=2]Explore[/wave][/font_size][/b]";
        addLevelsFolderButton.Visible = false;

        //load explore levels
        levelsList.ClearLevels();
        levelsList.SetLevels(await GameManager.Instance.BrowseLevelsFromAPI());
    }

    public async void MyLevels()
    {
        //set ui to my levels menu
        searchBar.AddThemeStyleboxOverride("panel", myLevelsStyle);
        searchBarText.Text = "[b][font_size=29][wave freq=2]My Levels[/wave][/font_size][/b]";
        addLevelsFolderButton.Visible = true;

        //load my levels
        levelsList.ClearLevels();
        levelsList.SetLevels(await GameManager.Instance.GetLevelsFromFolder());
    }

    public void ShowLevelsList(bool doShow)
    {
        searchBar.Visible = doShow;
        levelsList.Visible = doShow;
        levelView.Visible = !doShow;
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
            int percentage = 0;
            if (level.TotalPlays != 0)
            {
                percentage = Convert.ToInt32(Mathf.Clamp((float)level.FinishedPlays / (float)level.TotalPlays * 100f, 0f, 100f));
            }

            //subtract finished plays from total plays
            playDNFPercentage.Text = $"{100 - percentage}%";

            playFinishPercentage.Text = $"{percentage}%";
            playBar.Value = percentage;
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

    void OnAddLevelsFolderButtonPressed()
    {
        if (!fileDialog.Visible) fileDialog.PopupCentered();
    }

    void OnLevelsFolderSelected(string directory)
    {
        GameManager.Instance.SetLevelsFolder(directory);
        MyLevels();
    }
}
