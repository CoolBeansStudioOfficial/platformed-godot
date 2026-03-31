using Godot;
using System;
using System.Threading.Tasks;

public partial class LevelsMenu : Control
{
    [Export] FileDialog fileDialog;
    [Export] Button addLevelsFolderButton;
    [Export] LevelsList levelsList;
    [Export] Control levelView;

    [ExportGroup("Search")]
    [Export] Panel searchBar;
    [Export] RichTextLabel searchBarText;
    [Export] StyleBoxTexture exploreStyle;
    [Export] StyleBoxTexture myLevelsStyle;

    [ExportGroup("Level Buttons")]
    [Export] Button backButton;
    [Export] Button playButton;
    [Export] Button remixButton;
    [Export] Button deleteButton;
    [Export] ConfirmationDialog deleteConfirmation;
    [Export] Button webButton;
    [Export] Button descriptionSubmitButton;
    [Export] Button descriptionCancelButton;

    [ExportGroup("Level Info")]
    [Export] LineEdit levelNameEdit;
    [Export] Label levelCreator;
    [Export] TextEdit levelDescriptionEdit;
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
        remixButton.Pressed += OnRemixButtonPressed;
        deleteButton.Pressed += OnDeleteButtonPressed;
        deleteConfirmation.Confirmed += OnDeleteConfirmed;
        webButton.Pressed += OnWebButtonPressed;
        levelNameEdit.TextSubmitted += OnLevelNameChanged;
        levelDescriptionEdit.TextChanged += OnLevelDescriptionChanged;
        descriptionSubmitButton.Pressed += OnDescriptionSubmitted;
        descriptionCancelButton.Pressed += OnDescriptionCanceled;
        Explore();
    }

    public async Task Explore()
    {
        //set ui to explore menu
        searchBar.AddThemeStyleboxOverride("panel", exploreStyle);
        searchBarText.Text = "[b][font_size=29][wave freq=2][rainbow sat=0.1 val=1 freq=0.25]Explore[/rainbow][/wave][/font_size][/b]";
        addLevelsFolderButton.Visible = false;

        //load explore levels
        levelsList.ClearLevels();
        var levels = (await GameManager.Instance.BrowseLevelsFromAPI()).Levels;
        //mark online levels
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].Tags.Add("online");
        }

        levelsList.SetLevels(levels);
    }

    public async Task MyLevels()
    {
        //set ui to my levels menu
        searchBar.AddThemeStyleboxOverride("panel", myLevelsStyle);
        searchBarText.Text = "[b][font_size=29][wave freq=2][rainbow sat=0.1 val=1 freq=0.25]My Levels[/rainbow][/wave][/font_size][/b]";
        addLevelsFolderButton.Visible = true;

        //load my levels
        levelsList.ClearLevels();
        var myLevels = await GameManager.Instance.GetMyLevelsFromAPI();

        //load levels from folder
        var folderLevels = await GameManager.Instance.preferences.GetLevelsFromFolder();

        if (myLevels is not null)
        {
            //mark online levels
            for (int i = 0; i < myLevels.Count; i++)
            {
                myLevels[i].Tags.Add("online");
            }

            if (folderLevels is not null)
            {
                //combine level lists
                myLevels.AddRange(folderLevels);
            }

            levelsList.SetLevels(myLevels);
        }
        else if (folderLevels is not null)
        {
            levelsList.SetLevels(folderLevels);
        }
        
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

        levelNameEdit.Text = level.Name;
        levelNameEdit.Editable = false;

        levelCreator.Text = level.Username;

        levelDescriptionEdit.Text = level.Description;
        levelDescriptionEdit.Editable = false;

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

        webButton.Visible = false;
        deleteButton.Visible = false;

        if (level.Tags is not null)
        {
            foreach (var tag in level.Tags)
            {
                if (tag is string t) if (t == "online")
                {
                    webButton.Visible = true;

                    //if level is owned by player
                    if (GameManager.Instance.IsLoggedIn()) if (level.Owner == (int)GameManager.Instance.preferences.GetPreference("user_id"))
                    {
                        deleteButton.Visible = true;

                        levelNameEdit.Editable = true;
                        levelDescriptionEdit.Editable = true;
                    }
                }
            }
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
    void OnRemixButtonPressed()
    {
        if (selectedLevel is not null)
        {
            GameManager.Instance.RemixLevel(selectedLevel);
        }
    }

    void OnBackButtonPressed()
    {
        ShowLevelsList(true);
    }

    void OnDeleteButtonPressed()
    {
        deleteConfirmation.PopupCentered();
    }

    void OnDeleteConfirmed()
    {
        GameManager.Instance.DeleteLevel(selectedLevel.Id);
        GameManager.Instance.ReturnToLevelsMenu();
    }

    void OnWebButtonPressed()
    {
        OS.ShellOpen($"https://platformed.jmeow.net/level/{selectedLevel.Id}");
    }

    void OnLevelNameChanged(string newText)
    {
        if (levelNameEdit.Text == selectedLevel.Name) return;
        else if (newText == string.Empty)
        {
            levelNameEdit.Text = selectedLevel.Name;
            UIManager.Instance.PopupNotification("You cannot submit an empty level name.");
        }
        else
        {
            selectedLevel.Name = newText;
            GameManager.Instance.EditLevelDetails(new()
            {
                Name = levelNameEdit.Text,
                Public = true,
                Description = selectedLevel.Description,
                LevelId = selectedLevel.Id,
            });
        }
    }

    void OnLevelDescriptionChanged()
    {
        descriptionSubmitButton.Visible = true;
        descriptionCancelButton.Visible = true;
    }

    void OnDescriptionSubmitted()
    {
        if (levelDescriptionEdit.Text == selectedLevel.Description)
        {
            descriptionSubmitButton.Visible = false;
            descriptionCancelButton.Visible = false;

            return;
        }
        else if (levelDescriptionEdit.Text == string.Empty)
        {
            levelDescriptionEdit.Text = selectedLevel.Description;
            UIManager.Instance.PopupNotification("You cannot submit an empty description.");
        }
        else
        {
            selectedLevel.Description = levelDescriptionEdit.Text;
            GameManager.Instance.EditLevelDetails(new()
            {
                Name = selectedLevel.Name,
                Public = true,
                Description = levelDescriptionEdit.Text,
                LevelId = selectedLevel.Id,
            });
        }
        
        descriptionSubmitButton.Visible = false;
        descriptionCancelButton.Visible = false;
    }

    private void OnDescriptionCanceled()
    {
        levelDescriptionEdit.Text = selectedLevel.Description;
        descriptionSubmitButton.Visible = false;
        descriptionCancelButton.Visible = false;
    }

    void OnAddLevelsFolderButtonPressed()
    {
        if (!fileDialog.Visible) fileDialog.PopupCentered();
    }

    void OnLevelsFolderSelected(string directory)
    {
        GameManager.Instance.preferences.SetLevelsFolder(directory);
        MyLevels();
    }
}
