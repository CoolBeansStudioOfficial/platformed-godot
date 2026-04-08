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
        wallJumping.Pressed += OnWalljumpingPressed;
	}

    private void OnWalljumpingPressed()
    {
        throw new NotImplementedException();
    }

    void OnCloseRequested()
    {
        Hide();
    }

    

    void OnResizePressed()
    {
        editor.ChangeLevelSize(new((int)width.Value, (int)height.Value));
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
