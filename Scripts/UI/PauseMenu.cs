using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export] Button continueButton;
    [Export] Button quitButton;

    public override void _Ready()
    {
        continueButton.Pressed += OnContinueButtonPressed;
        quitButton.Pressed += OnQuitButtonPressed;
    }

    void OnContinueButtonPressed()
    {
        Visible = false;
    }

    void OnQuitButtonPressed()
    {
        GameManager.Instance.ReturnToLevelView();

        Visible = false;
    }
}
