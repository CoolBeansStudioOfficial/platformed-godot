using Godot;
using System;

public partial class EditorSettings : Window
{
    [Export] Editor editor;

    
    [Export] Button close;
    [Export] SpinBox width;
    [Export] SpinBox height;
    [Export] Button resize;
    [Export] OptionButton wallJumping;

    public override void _Ready()
	{
        close.Pressed += OnCloseRequested;
        CloseRequested += OnCloseRequested;

        resize.Pressed += OnResizePressed;
        wallJumping.ItemSelected += OnWalljumpingPressed;
	}

    void OnCloseRequested()
    {
        Hide();
    }

    void OnResizePressed()
    {
        editor.ChangeLevelSize(new((int)width.Value, (int)height.Value));
    }

    void OnWalljumpingPressed(long index)
    {
        if (index == 0) editor.currentLevel.Data.WallJump = "";
        else if (index == 1) editor.currentLevel.Data.WallJump = "up";
        else editor.currentLevel.Data.WallJump = "off";
    }

    public void ApplySettings(Level level)
    {
        width.Value = level.Width; 
        height.Value = level.Height;

        if (level.Data.WallJump == "up") wallJumping.Select(1);
        else if (level.Data.WallJump == "off") wallJumping.Select(2);
        else wallJumping.Select(0);

        PopupCentered();
    }
}
