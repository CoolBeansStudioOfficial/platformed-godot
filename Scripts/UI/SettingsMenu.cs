using Godot;
using Godot.Collections;
using System;

public partial class SettingsMenu : Control
{
    [ExportGroup("Controls")]
    [Export] Window window;
    [Export] Button apply;
    [Export] Button close;
    [Export] Button ok;

    [ExportGroup("Settings Buttons")]
	[Export] OptionButton styleOptions;

    [ExportGroup("Config")]
    [Export] TileSet[] playSets;
    [Export] TileSet[] editorSets;

    Dictionary<string, Variant> savedSettings;
    Dictionary<string, Variant> unsavedSettings;

    public override void _Ready()
    {
        window.CloseRequested += OnCloseRequested;
        window.AboutToPopup += GetSettings;
        apply.Pressed += Apply;
        close.Pressed += Close;
        ok.Pressed += OK;

        GetSettings();
        ApplySettings();
    }

    void GetSettings()
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
        //set values
        unsavedSettings["tile_style"] = styleOptions.GetSelectedId();

        //save to config file
        GameManager.Instance.SetPreference("app_settings", unsavedSettings);

        //reset saved settings
        savedSettings = (Dictionary<string, Variant>)GameManager.Instance.GetPreference("app_settings");

        ApplySettings();
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

    void ApplySettings()
    {
        //apply settings
        LevelManager.Instance.SetTileset(playSets[(int)savedSettings["tile_style"]]);
        UIManager.Instance.editor.SetTileset(editorSets[(int)savedSettings["tile_style"]]);
    }

    private void OnCloseRequested()
    {
        Close();
    }
}
