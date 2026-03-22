using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] public Vector2I gridSize;
    [Export] TilesViewport viewport;
    [Export] TileMapLayer tileMap;
    [Export] TileMapLayer overlayMap;

    [ExportGroup("UI")]
    [Export] LineEdit nameEdit;
    [Export] Label unsavedLabel;
    [Export] Button backButton;
    [Export] HBoxContainer flyoutOptions;
    [Export] Button confirmButton;
    [Export] Button cancelButton;
    [Export] Button playButton;
    [Export] Button undoButton;
    [Export] Button redoButton;
    [Export] MenuButton saveButton;

    Level currentLevel;

    List<List<List<TileInfo>>> editHistory = [];
    int currentEdit = 0;
    bool placeMode = false;
    bool mouseDown = false;

    public bool eraserSelected = false;
    TileId selectedTile = TileId.Ground;

    public override void _Ready()
    {
        backButton.Pressed += OnBackButtonPressed;
        nameEdit.TextChanged += NameChanged;
        confirmButton.Pressed += Confirm;
        cancelButton.Pressed += Cancel;
        playButton.Pressed += Play;
        undoButton.Pressed += Undo;
        redoButton.Pressed += Redo;
        saveButton.GetPopup().IdPressed += SaveLevel;

        ResetEditHistory();

        viewport.GuiInput += OnViewportInput;

        viewport.viewportSize = gridSize * 16;
        for (int i = 0; i < gridSize.Y; i++)
        {
            //add row
            editHistory[currentEdit].Add([]);

            //add tiles to row
            for (int j = 0; j < gridSize.X; j++)
            {
                editHistory[currentEdit][i].Add(new());
            }
        }
    }

    void OnViewportInput(InputEvent @event)
    {
        Vector2I mouseCoords = tileMap.LocalToMap(tileMap.GetLocalMousePosition());

        //mouse button presses
        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                //handle left mouse button press
                if (mb.Pressed)
                {
                    mouseDown = true;

                    //place mode
                    if (placeMode)
                    {
                        if (eraserSelected) SetTile(mouseCoords, TileId.Air);
                        else SetTile(mouseCoords, selectedTile);
                    }
                }
                //handle left mouse button release
                else
                {
                    mouseDown = false;
                }
            }
        }
        //mouse movement
        else if (@event is InputEventMouseMotion)
        {
            if (mouseDown)
            {
                if (placeMode)
                {
                    if (eraserSelected) SetTile(mouseCoords, TileId.Air);
                    else SetTile(mouseCoords, selectedTile);
                }
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        //key presses
        if (@event is InputEventKey k)
        {
            if (k.Pressed)
            {
                if (k.Keycode == Key.Z && k.IsCommandOrControlPressed())
                {
                    if (k.ShiftPressed) Redo();
                    else Undo();
                }
            }
        }
    }

    void Undo()
    {
        if (currentEdit == 0) return;

        //if in place mode, exit place mode before reverting changes
        if (placeMode) SetMode(EditorMode.Edit);
        
        RevertEdit();
    }

    void Redo()
    {
        if (currentEdit == editHistory.Count - 1) return;
        RedoEdit();
    }

    void Cancel()
    {
        if (placeMode) Undo();
    }

    void Confirm()
    {
        if (placeMode) SetMode(EditorMode.Edit);
    }

    void SetTile(Vector2I position, TileId id)
    {
        //return if position is outside of set level size
        if (position.X >= gridSize.X || position.Y >= gridSize.Y) return;
        if (position.X < 0 || position.Y < 0) return;

        if (id == TileId.Air) tileMap.SetCell(position);
        else tileMap.SetCell(position, (int)id, Vector2I.Zero);

        TileInfo editedTile = editHistory[currentEdit][position.Y][position.X];
        editedTile.id = id;
        editHistory[currentEdit][position.Y][position.X] = editedTile;

        UpdateTiles();
    }

    enum EditorMode
    {
        Edit,
        Place
    }

    void SetMode(EditorMode mode)
    {
        //place mode
        if (mode == EditorMode.Place)
        {
            placeMode = true;
            flyoutOptions.Visible = true;

            AddEdit();
        }
        //edit mode
        else
        {
            placeMode = false;
            eraserSelected = false;
            flyoutOptions.Visible = false;
        }
    }

    void AddEdit()
    {
        List<List<TileInfo>> newEdit = [];
        newEdit.AddRange(editHistory[currentEdit]);

        //remove all edits after the current one
        editHistory.RemoveRange(currentEdit + 1, editHistory.Count - currentEdit - 1);

        //start new edit
        editHistory.Add(newEdit);
        currentEdit++;

        UpdateTiles();

        undoButton.Disabled = false;
        redoButton.Disabled = true;
    }

    void RevertEdit()
    {
        currentEdit--;
        UpdateTiles();

        if (currentEdit == 0) undoButton.Disabled = true;
        redoButton.Disabled = false;
    }

    void RedoEdit()
    {
        currentEdit++;
        UpdateTiles();

        if (currentEdit == editHistory.Count - 1) redoButton.Disabled = true;
        undoButton.Disabled = false;
    }

    void ResetEditHistory()
    {
        editHistory.Clear();
        editHistory.Add([]);
        currentEdit = 0;
    }

    public void SelectTile(TileId id)
    {
        if (!placeMode) SetMode(EditorMode.Place);

        selectedTile = id;
    }

    public void SelectEraser(bool doSelect)
    {
        if (!eraserSelected && !placeMode && doSelect) SetMode(EditorMode.Place);

        eraserSelected = doSelect;
    }



    public void UpdateTiles()
    {
        editHistory[currentEdit] = LevelManager.Instance.UpdateAdjacencies(editHistory[currentEdit]);
        
        //set cells according to stored tiles
        foreach (var row in editHistory[currentEdit])
        {
            foreach (var tile in row)
            {
                tileMap.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }
    

    public void ImportLevel(Level level)
	{
        currentLevel = level;
        nameEdit.Text = level.Name;

        ResetEditHistory();

        editHistory[currentEdit] = LevelManager.Instance.CreateTilemap(level);

        gridSize = new(level.Width, level.Height);

        tileMap.Clear();
        foreach (var row in editHistory[currentEdit])
        {
            foreach (var tile in row)
            {
                tileMap.SetCell(tile.position, (int)tile.id, LevelManager.Instance.GetAtlasCoords(tile));
            }
        }
    }

    private void SaveLevel(long id)
    {
        //save to levels folder
        if (id == 0)
        {
            if (GameManager.Instance.IsLevelsFolderSet())
            {
                currentLevel.Data.Layers[0].Data = LevelManager.Instance.EncodeRLE(editHistory[currentEdit]);

                string path = $"{GameManager.Instance.GetLevelsFolder()}/{currentLevel.Name}.json";
                GameManager.Instance.SaveLevelAsFile(currentLevel, path);
            }
            else
            {
                UIManager.Instance.PopupNotification("No levels folder has been set\n(You can choose a folder in the My Levels menu)", "Save Failed");
            }

            
        }
        //save to custom directory
        else if (id == 1)
        {
            GD.Print("save to custom directory");
        }
        //upload to web
        else
        {
            UIManager.Instance.PopupNotification("I have't actually implemented this yet lol", "Upload Failed");
        }
    }

    private void NameChanged(string newText)
    {
        currentLevel.Name = newText;
    }

    void Play()
    {
        currentLevel.Data.Layers[0].Data = LevelManager.Instance.EncodeRLE(editHistory[currentEdit]);
        GameManager.Instance.PlayLevel(currentLevel);
    }

    void OnBackButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu();

        //other editor exitng stuff can be handled here
    }
}
