using Godot;
using System;

public partial class LevelPreview : Panel
{
    [Export] Button playButton;

    [Export] Label levelName;
    [Export] Label creator;
    [Export] Label likePercentage;
    [Export] Label completionPercentage;

    Level level;

    public override void _Ready()
	{
        playButton.Pressed += OnPlayButtonPressed;
	}

    public void SetLevel(Level newLevel)
    {
        level = newLevel;

        levelName.Text = level.Name;
        creator.Text = level.Owner.ToString();
        likePercentage.Text = $"{level.ApprovalPercentage}%";
        int percentage = Mathf.Clamp(Convert.ToInt32(((float)level.FinishedPlays / (float)level.TotalPlays) * 100f), 0, 100);
        completionPercentage.Text = $"{percentage}%";
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	void OnPlayButtonPressed()
    {
        GameManager.Instance.PlayLevel(level);
    }
}
