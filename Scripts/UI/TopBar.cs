using Godot;
using System;

public partial class TopBar : Panel
{
    [Export] Button createButton;
    [Export] Button exploreButton;
    [Export] Button myLevelsButton;

    public override void _Ready()
	{
        createButton.Pressed += OnCreateButtonPressed;
        exploreButton.Pressed += OnExploreButtonPressed;
        myLevelsButton.Pressed += OnMyLevelsButtonPressed;
	}

	void OnCreateButtonPressed()
    {
        UIManager.Instance.editor.Visible = true;
        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.pauseMenu.Visible = false;
        LevelManager.Instance.DestroyLevel();
    }

    void OnExploreButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.Explore();
        UIManager.Instance.editor.Visible = false;
    }

    void OnMyLevelsButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.MyLevels();
        UIManager.Instance.editor.Visible = false;
    }
}
