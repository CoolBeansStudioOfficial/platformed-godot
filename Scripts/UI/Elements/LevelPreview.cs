using Godot;
using System;

public partial class LevelPreview : Panel
{
    [Export] Button playButton;

    [Export] Label levelName;
    [Export] Label creator;
    [Export] Label likePercentage;
    [Export] Label completionPercentage;
    [Export] Panel cloudIcon;

    public Level level;

    public override void _Ready()
	{
        playButton.Pressed += OnPlayButtonPressed;
	}

    public void SetLevel(Level newLevel)
    {
        level = newLevel;

        levelName.Text = level.Name;
        creator.Text = level.Username;
        likePercentage.Text = $"{level.ApprovalPercentage}%";
        int percentage = 0;
        if (level.TotalPlays != 0)
        {
            percentage = Convert.ToInt32(Mathf.Clamp((float)level.FinishedPlays / (float)level.TotalPlays * 100f, 0f, 100f));
        }
        
        completionPercentage.Text = $"{percentage}%";

        if (level.Tags is not null)
        {
            foreach (var tag in level.Tags)
            {
                if (tag is string t)
                {
                    if (t == "online") cloudIcon.Visible = true;
                }
            }
        }
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	void OnPlayButtonPressed()
    {
        UIManager.Instance.levelsMenu.ViewLevel(level);
    }
}
