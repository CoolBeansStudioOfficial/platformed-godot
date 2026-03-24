using Godot;
using Godot.Collections;
using System;

public partial class SettingsMenu : Control
{
    [Export] Window window;
    [Export] Button apply;
    [Export] Button close;
    [Export] Button ok;

	[Export] OptionButton styleOptions;

    Dictionary<string, Variant> savedSettings;
    Dictionary<string, Variant> unsavedSettings;

    public override void _Ready()
    {
        window.CloseRequested += OnCloseRequested;
        window.AboutToPopup += OnShow;
        apply.Pressed += Apply;
        close.Pressed += Close;
        ok.Pressed += OK;
    }

    void OnShow()
    {
        savedSettings = (Dictionary<string, Variant>)GameManager.Instance.GetPreference("app_settings");

        //create new default settings config if settings has never been set before
        if (savedSettings == null)
        {
            GameManager.Instance.SetPreference("app_settings", new Dictionary<string, Variant>()
            {
                { "tile_style", 1 }
            });
        }

        savedSettings = (Dictionary<string, Variant>)GameManager.Instance.GetPreference("app_settings");
        unsavedSettings = (Dictionary<string, Variant>)GameManager.Instance.GetPreference("app_settings");

        //set buttons to match config file
        styleOptions.Selected = (int)savedSettings["tile_style"];
    }

    public void Apply()
    {
        unsavedSettings["tile_style"] = styleOptions.GetSelectedId();

        GameManager.Instance.SetPreference("app_settings", unsavedSettings);
    }

    public void OK()
    {
        Apply();
        Close();
    }

    public void Close()
    {
        window.Hide();
    }

    private void OnCloseRequested()
    {
        Close();
    }
}
