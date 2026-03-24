using Godot;
using System;

public partial class SettingsMenu : Control
{
    [Export] Window window;
	[Export] OptionButton styleOptions;

    public override void _Ready()
    {
        window.CloseRequested += OnCloseRequested;
        styleOptions.ItemSelected += OnStyleChanged;
    }


    void OnStyleChanged(long index)
    {
        //8 bit
        if (index == 0)
        {
            GameManager.Instance.SetPreference("tile_style", 0);
        }
        //16 bit
        else if (index == 1)
        {
            GameManager.Instance.SetPreference("tile_style", 0);
        }
    }

    private void OnCloseRequested()
    {
        window.Hide();
    }
}
