using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class SettingsMenu : Control
{
    [ExportGroup("Controls")]
    [Export] Window window;
    [Export] ConfirmationDialog unsavedDialog;
    [Export] Button apply;
    [Export] Button close;
    [Export] Button ok;

    [ExportGroup("Settings Buttons")]
    [Export] ColorPickerButton backgroundColor;
    [Export] ColorPickerButton accentColor;
    [Export] OptionButton styleOptions;

    [ExportGroup("Config")]
    [Export] TileSet[] playSets;
    [Export] TileSet[] editorSets;

    Dictionary<string, Variant> settings;

    public override void _Ready()
    {
        window.CloseRequested += OnCloseRequested;
        unsavedDialog.Confirmed += UnsavedDialogConfirmed;
        unsavedDialog.Canceled += UnsavedDialogCanceled;
        window.AboutToPopup += GetSettings;
        apply.Pressed += Apply;
        close.Pressed += Close;
        ok.Pressed += OK;

        GetSettings();
        Apply();
    }

    void GetSettings()
    {
        settings = (Dictionary<string, Variant>)GameManager.Instance.preferences.GetPreference("app_settings");

        //create new default settings config if settings has never been set before
        if (settings == null)
        {
            GameManager.Instance.preferences.SetPreference("app_settings", new Dictionary<string, Variant>()
            {
                { "tile_style", 1 },
                { "background_color", new Color("#222222") },
                { "accent_color", new Color("#94daf2") }
            });
        }

        settings = (Dictionary<string, Variant>)GameManager.Instance.preferences.GetPreference("app_settings");

        //set buttons to match config file
        styleOptions.Selected = (int)settings["tile_style"];
        backgroundColor.Color = (Color)settings["background_color"];
        accentColor.Color = (Color)settings["accent_color"];
    }

    public void Update()
    {
        //set values
        settings["tile_style"] = styleOptions.GetSelectedId();
        settings["background_color"] = backgroundColor.Color;
        settings["accent_color"] = accentColor.Color;

    }

    void Apply()
    {
        Update();

        //save to config file
        GameManager.Instance.preferences.SetPreference("app_settings", settings);

        //apply settings
        LevelManager.Instance.SetTileset(playSets[(int)settings["tile_style"]]);
        SetEditorTileset();
        
        ThemeManager.Instance.backgroundColor = (Color)settings["background_color"];
        ThemeManager.Instance.accentColor = (Color)settings["accent_color"];
        ThemeManager.Instance.SetTheme();
    }

    //i only did this because it was giving an error when apply is called in ready
    public async Task SetEditorTileset()
    {
        try
        {
            UIManager.Instance.editor.SetTileset(editorSets[(int)settings["tile_style"]]);
        }
        catch
        {
            await Task.Delay(1000);
            UIManager.Instance.editor.SetTileset(editorSets[(int)settings["tile_style"]]);
        }
    }

    public void OK()
    {
        Apply();
        window.Hide();
    }

    public void Close()
    {
        Update();
        bool unsavedChanges = true;
        if (DoSettingsMatch(settings, (Dictionary<string, Variant>)GameManager.Instance.preferences.GetPreference("app_settings"))) unsavedChanges = false;

        if (!unsavedChanges) window.Hide();
        else
        {
            unsavedDialog.PopupCentered();
        }
    }


    void UnsavedDialogConfirmed()
    {
        OK();
    }

    void UnsavedDialogCanceled()
    {
        window.Hide();
    }


    void OnCloseRequested()
    {
        Close();
    }
    
    bool DoSettingsMatch(Dictionary<string, Variant> d1, Dictionary<string, Variant> d2)
    {
        foreach (var item in d1)
        {
            if (!item.Value.Equals(d2[item.Key])) return false;
        }

        return true;
    }
}
