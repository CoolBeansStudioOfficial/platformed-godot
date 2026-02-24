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

    }

    void OnExploreButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.Explore();
    }

    void OnMyLevelsButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu(true);
        UIManager.Instance.levelsMenu.MyLevels();
    }
}
