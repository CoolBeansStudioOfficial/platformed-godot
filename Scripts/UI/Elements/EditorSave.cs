using Godot;
using System;

public partial class EditorSave : MenuButton
{
    [Export] FileDialog saveDialog;
    [Export] ConfirmationDialog uploadDialog;
    [Export] Label unsavedLabel;

    public override void _Ready()
	{
        GetPopup().IdPressed += SaveLevel;
        saveDialog.FileSelected += SaveDialogFileSelected;
        uploadDialog.Confirmed += UploadDialogConfirmed;
    }

    public override void _Input(InputEvent @event)
    {
        //key presses
        if (@event is InputEventKey k)
        {
            if (k.Pressed)
            {
                if (k.Keycode == Key.S && k.IsCommandOrControlPressed())
                {
                    SaveLevel(0);
                }
            }
        }
    }

    void SaveLevel(long id)
    {
        Level currentLevel = UIManager.Instance.editor.currentLevel;
        var currentEdit = UIManager.Instance.editor.GetCurrentEdit();

        //save changes to working level class instance
        currentLevel.Data.Layers[0].Data = LevelManager.Instance.EncodeRLE(currentEdit);
        currentLevel.Data.Layers[1].Data = LevelManager.Instance.EncodeRLE(currentEdit, LevelManager.EncodeFilter.Rotation);

        //save to levels folder
        if (id == 0)
        {
            if (GameManager.Instance.preferences.IsLevelsFolderSet())
            {
                string path = $"{GameManager.Instance.preferences.GetLevelsFolder()}/{currentLevel.Name}.json";
                GameManager.Instance.preferences.SaveLevelAsFile(currentLevel, path);

                unsavedLabel.Visible = false;
            }
            else
            {
                UIManager.Instance.PopupNotification("No levels folder has been set\n(You can choose a folder in the My Levels menu)", "Save Failed");
            }


        }
        //save to custom directory
        else if (id == 1)
        {
            saveDialog.FileNameFilter = currentLevel.Name;
            saveDialog.Popup();
        }
        //upload to web
        else
        {
            if (GameManager.Instance.IsLoggedIn())
            {
                uploadDialog.DialogText = "Are you sure you want to publicly upload this level to your account?\n(You can edit it later in the web editor)";
                uploadDialog.OkButtonText = "Upload";

                if (currentLevel.Tags is not null) foreach (var tag in currentLevel.Tags)
                {
                    if (tag is string t) if (t == "online")
                    {
                        uploadDialog.DialogText = "Are you sure you want to overwrite your existing online level?\nThis CANNOT be undone!";
                        uploadDialog.OkButtonText = "Update";
                    }
                }

                uploadDialog.PopupCentered();
            }
            else
            {
                UIManager.Instance.PopupNotification("You are not currently signed in. Please sign in to upload levels.", "Upload Failed");
            }

        }
    }

    void SaveDialogFileSelected(string path)
    {
        Level currentLevel = UIManager.Instance.editor.currentLevel;
        GameManager.Instance.preferences.SaveLevelAsFile(currentLevel, path);
        unsavedLabel.Visible = false;
    }

    void UploadDialogConfirmed()
    {
        Level currentLevel = UIManager.Instance.editor.currentLevel;
        currentLevel.Description = $"This level was uploaded by {GameManager.Instance.preferences.GetPreference("username")} using the desktop client!";
        currentLevel.Data.TilesetPath = "/assets/medium.json";

        //edit level instead if this is a change to an existing level
        if (currentLevel.Tags is not null) foreach (var tag in currentLevel.Tags)
        {
            if (tag is string t) if (t == "online")
            {
                GameManager.Instance.EditLevel(currentLevel, currentLevel.Id);
                return;
            }
        }
        currentLevel.Data.Layers[1].Name = "rotation";

        GameManager.Instance.UploadLevel(currentLevel);
    }
}
