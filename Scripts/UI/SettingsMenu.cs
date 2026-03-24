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

        }
        //16 bit
        else if (index == 1)
        {

        }
    }

    private void OnCloseRequested()
    {
        window.Hide();
    }
}
