using Godot;
using System;

public partial class TopBar : Panel
{
    [Export] Button createButton;
    [Export] Button returnToEditorButton;
    [Export] Button exploreButton;
    [Export] Button myLevelsButton;

    public override void _Ready()
	{
        createButton.Pressed += OnCreateButtonPressed;
        returnToEditorButton.Pressed += OnCreateButtonPressed;
        exploreButton.Pressed += OnExploreButtonPressed;
        myLevelsButton.Pressed += OnMyLevelsButtonPressed;
	}

	void OnCreateButtonPressed()
    {
        returnToEditorButton.Visible = false;
        UIManager.Instance.editor.Visible = true;
        UIManager.Instance.levelsMenu.Visible = false;
        UIManager.Instance.pauseMenu.Visible = false;
        LevelManager.Instance.DestroyLevel();
    }

    void OnExploreButtonPressed()
    {
        returnToEditorButton.Visible = false;
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.Explore();
        UIManager.Instance.editor.Visible = false;
    }

    void OnMyLevelsButtonPressed()
    {
        returnToEditorButton.Visible = false;
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.MyLevels();
        UIManager.Instance.editor.Visible = false;
    }
}
