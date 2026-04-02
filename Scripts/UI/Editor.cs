using Godot;
using System;
using System.Collections.Generic;

public partial class Editor : Control
{
    [Export] public Vector2I gridSize;
    [Export] TilesViewport viewport;
    [Export] TileMapLayer tileMap;

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
    [Export] EditorOverlay overlay;
    [Export] EditorContextMenu contextMenu;

    [ExportGroup("Block Textures")]
    [Export] Godot.Collections.Dictionary<TileId, Texture2D> textures;
    [Export] Rect2 textureRegion;

    public Level currentLevel;

    List<List<List<TileInfo>>> editHistory = [];
    int currentEdit = 0;
    bool placeMode = false;
    bool mouseDown = false;

    //place mode
    public bool eraserSelected = false;
    TileId selectedTile;

    //edit mode
    bool startedSelection = false;
    bool startedDrag = false;
    List<TileInfo> dragInitial = [];
    EditSelection selection;

    public struct EditSelection
    {
        public Vector2I start;
        public Vector2I end;
        public Vector2I dragPosition;
        public List<TileInfo> tiles;

        //move selection to point relative to drag position
        public void Move(Vector2I position)
        {
            Vector2I offset = position - dragPosition;

            //offset selection positions
            start += offset;
            end += offset;
            dragPosition += offset;

            //offset stored tile positions
            for (int i = 0; i < tiles.Count; i++)
            {
                TileInfo tile = tiles[i];
                tile.position += offset;
                tiles[i] = tile;
            }
        }

        public bool IsInRect(Vector2I position)
        {
            Vector2 rangeX = new(Mathf.Min(start.X, end.X), Mathf.Max(start.X, end.X));
            Vector2 rangeY = new(Mathf.Min(start.Y, end.Y), Mathf.Max(start.Y, end.Y));

            //return false if horizontally out of bounds
            if (!(position.X >= rangeX.X && position.X <= rangeX.Y)) return false;

            //return false if vertically out of bounds
            if (!(position.Y >= rangeY.X && position.Y <= rangeY.Y)) return false;

            return true;
        }

        public List<Vector2I> GetCells()
        {
            List<Vector2I> cells = [];

            for (int y = Mathf.Min(start.Y, end.Y); y <= Mathf.Max(start.Y, end.Y); y++)
            {
                for (int x = Mathf.Min(start.X, end.X); x <= Mathf.Max(start.X, end.X); x++)
                {
                    cells.Add(new(x, y));
                }
            }

            return cells;
        }

        public Vector2I GetCenter()
        {
            return new((start.X + end.X) / 2, (start.Y + end.Y) / 2);
        }

        public Vector2 GetSize()
        {
            return new(Mathf.Abs(start.X - end.X) + 1, Mathf.Abs(start.Y - end.Y) + 1);
        }
    }


    public override void _Ready()
    {
        backButton.Pressed += OnBackButtonPressed;
        nameEdit.TextChanged += NameChanged;
        confirmButton.Pressed += Confirm;
        cancelButton.Pressed += Cancel;
        playButton.Pressed += Play;
        undoButton.Pressed += Undo;
        redoButton.Pressed += Redo;

        contextMenu.OptionPressed += OnContextMenuPressed; ;

        ResetEditHistory();

        viewport.GuiInput += OnViewportInput;

        //add
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

        //create default level
        currentLevel = new()
        {
            Data = new()
            {
                Spawn = new()
                {
                    X = 0,
                    Y = 0,
                },
                Width = gridSize.X,
                Height = gridSize.Y,
                Layers = [new()
                {
                    Data = [],
                    Name = "level",
                    Type = "tileLayer"
                }, new()
                {
                    Data = [],
                    Type = "rotation"
                }],
                WallJump = "off"
            },
            Name = "Untitled",
            CreatedAt = DateTime.Now,
            Width = gridSize.X,
            Height = gridSize.Y,
        };
    }

    public void SetTileset(TileSet tileset)
    {
        tileMap.TileSet = tileset;
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

                    HandleClick(mouseCoords);
                }
                //handle left mouse button release
                else
                {
                    mouseDown = false;

                    //if started selection, get blocks in selection and end it
                    if (startedSelection)
                    {
                        //add tile data to selection
                        foreach (Vector2I cell in selection.GetCells())
                        {
                            if (!IsInGrid(cell)) continue;
                            TileInfo tile = editHistory[currentEdit][cell.Y][cell.X];
                            selection.tiles.Add(tile);
                        }

                        //show context menu
                        contextMenu.Popup();

                        startedSelection = false;
                    }
                    //if selection was being dragged, handle release
                    else if (startedDrag)
                    {
                        AddEdit();

                        //remove tiles from previous position
                        foreach (TileInfo tile in dragInitial)
                        {
                            if (tile.id == TileId.Air) continue;
                            SetTile(tile.position, TileId.Air, tile.rotation, false);
                        }

                        //add tiles to new position
                        foreach (TileInfo tile in selection.tiles)
                        {
                            if (tile.id == TileId.Air) continue;
                            SetTile(tile.position, tile.id, tile.rotation, false);
                        }
                        UpdateTiles();

                        GD.Print("selection moved");
                        startedDrag = false;
                    }
                }
            }
        }
        //mouse movement
        else if (@event is InputEventMouseMotion)
        {
            if (placeMode && !eraserSelected)
            {
                Vector2 worldPosition = tileMap.MapToLocal(mouseCoords);
                Vector2 boxSize = new(16, 16);

                overlay.SetTextures(
                [
                    new() {
                    texture = textures[selectedTile],
                    rect = new Rect2(worldPosition.X - boxSize.X / 2, worldPosition.Y - boxSize.Y / 2, boxSize.X, boxSize.Y),
                    region = textureRegion,
                    color = Colors.White,
                    }
                ]);
            }
            else
            {
                overlay.SetTextures(null);
            }

            if (mouseDown)
            {
                HandleClick(mouseCoords, true);
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
                if (k.Keycode == Key.Space && k.IsCommandOrControlPressed())
                {
                    Play();
                }

                if (k.Keycode == Key.Z && k.IsCommandOrControlPressed())
                {
                    if (k.ShiftPressed) Redo();
                    else Undo();
                }
            }
        }
    }

    void HandleClick(Vector2I mouseCoords, bool drag = false)
    {
        //place mode
        if (placeMode)
        {
            if (eraserSelected) SetTile(mouseCoords, TileId.Air);
            else
            {
                //remove any other spawn tiles
                if (selectedTile == TileId.Spawn)
                {
                    for (int y = 0; y < editHistory[currentEdit].Count; y++)
                    {
                        for (int x = 0; x < editHistory[currentEdit][y].Count; x++)
                        {
                            if (editHistory[currentEdit][y][x].id == TileId.Spawn)
                            {
                                SetTile(new(x, y), TileId.Air);
                            }
                        }
                    }

                    //set level spawn
                    currentLevel.Data.Spawn = new() 
                    { 
                        X = mouseCoords.X,
                        Y = mouseCoords.Y,
                    };
                }

                SetTile(mouseCoords, selectedTile);
            }

            overlay.SetOutline(null);
        }
        //edit mode
        else
        {
            //click
            if (!drag)
            {
                //if selection has not been started, start new selection
                if (!startedSelection && !selection.IsInRect(mouseCoords))
                {
                    selection = new()
                    {
                        start = mouseCoords,
                        end = mouseCoords,
                        tiles = []
                    };

                    startedSelection = true;
                }
            }
            //drag
            else
            {
                if (startedSelection)
                {
                    selection.end = mouseCoords;
                }
                else
                {
                    if (startedDrag)
                    {
                        selection.Move(mouseCoords);

                        //update overlay
                        Vector2 overlaySize = new(16, 16);
                        List<EditorOverlay.Outline?> movePreview = [];
                        foreach (var tile in selection.tiles)
                        {
                            if (tile.id == TileId.Air) continue;

                            Vector2 overlayPosition = tileMap.MapToLocal(tile.position);

                            movePreview.Add(new()
                            {
                                texture = textures[tile.id],
                                rect = new Rect2(overlayPosition.X - overlaySize.X / 2, overlayPosition.Y - overlaySize.Y / 2, overlaySize.X, overlaySize.Y),
                                region = textureRegion,
                                color = Colors.White,
                            });
                        }

                        overlay.SetTextures(movePreview);
                    }
                    else
                    {
                        //if the mouse is inside of selection box, drag selection
                        if (selection.IsInRect(mouseCoords))
                        {
                            selection.dragPosition = mouseCoords;

                            dragInitial.Clear();
                            foreach (Vector2I cell in selection.GetCells())
                            {
                                if (!IsInGrid(cell)) continue;
                                TileInfo tile = editHistory[currentEdit][cell.Y][cell.X];
                                dragInitial.Add(tile);
                            }

                            startedDrag = true;
                        }
                    }
                }
            }

            Vector2 worldPosition = tileMap.MapToLocal(selection.GetCenter());

            //if selection width is even, move the box to the right by half a tile
            if ((int)selection.GetSize().X % 2 == 0 && (int)selection.GetSize().X > 1)
            {
                worldPosition.X += 8f;
            }

            //if selection height is even, move the box to the down by half a tile
            if ((int)selection.GetSize().Y % 2 == 0 && (int)selection.GetSize().Y > 1)
            {
                worldPosition.Y += 8f;
            }

            Vector2 boxSize = selection.GetSize() * 16;

            overlay.SetOutline(new()
            {
                rect = new Rect2(worldPosition.X - (boxSize.X / 2), worldPosition.Y - (boxSize.Y / 2), boxSize.X, boxSize.Y),
                color = Colors.Orange,
                width = 1,
            });

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

    void SetTile(Vector2I position, TileId id, TileRotation rotation = TileRotation.Up, bool updateTiles = true)
    {
        //return if position is outside of set level size
        if (!IsInGrid(position)) return;

        if (id == TileId.Air) tileMap.SetCell(position);
        else tileMap.SetCell(position, (int)id, Vector2I.Zero);

        TileInfo editedTile = editHistory[currentEdit][position.Y][position.X];
        editedTile.id = id;
        editedTile.rotation = rotation;
        editHistory[currentEdit][position.Y][position.X] = editedTile;

        if (updateTiles) UpdateTiles();
    }

    bool IsInGrid(Vector2I position)
    {
        if (position.X >= gridSize.X || position.Y >= gridSize.Y) return false;
        if (position.X < 0 || position.Y < 0) return false;
        return true;
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

            ResetSelection();

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

    void ResetSelection()
    {
        selection = default;
        overlay.currentOutlines[0] = null;
        overlay.QueueRedraw();
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

        unsavedLabel.Visible = true;
    }

    void RevertEdit()
    {
        currentEdit--;
        UpdateTiles();

        if (currentEdit == 0) undoButton.Disabled = true;
        redoButton.Disabled = false;

        unsavedLabel.Visible = true;

        ResetSelection();
    }

    void RedoEdit()
    {
        currentEdit++;
        UpdateTiles();

        if (currentEdit == editHistory.Count - 1) redoButton.Disabled = true;
        undoButton.Disabled = false;

        unsavedLabel.Visible = true;
    }

    void ResetEditHistory()
    {
        editHistory.Clear();
        editHistory.Add([]);
        currentEdit = 0;
    }

    public List<List<TileInfo>> GetCurrentEdit()
    {
        return editHistory[currentEdit];
    }

    public void SelectTile(TileId newSelection)
    {
        if (!placeMode) SetMode(EditorMode.Place);

        selectedTile = newSelection;
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

    void OnContextMenuPressed()
    {
        var option = contextMenu.selectedOption;
        GD.Print(contextMenu.selectedOption);

        if (option == EditorContextMenu.Option.Fill)
        {

        }
        else if (option == EditorContextMenu.Option.Copy)
        {

        }
        else if (option == EditorContextMenu.Option.Cut)
        {

        }
        else if (option == EditorContextMenu.Option.Paste)
        {

        }
    }

    public void ImportLevel(Level level)
	{
        currentLevel = level;
        nameEdit.Text = level.Name;

        ResetEditHistory();
        unsavedLabel.Visible = true;

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

    private void NameChanged(string newText)
    {
        currentLevel.Name = newText;
    }

    void Play()
    {
        currentLevel.Data.Layers[0].Data = LevelManager.Instance.EncodeRLE(editHistory[currentEdit]);
        currentLevel.Data.Layers[1].Data = LevelManager.Instance.EncodeRLE(editHistory[currentEdit], LevelManager.EncodeFilter.Rotation);
        GameManager.Instance.PlayLevel(currentLevel);
        UIManager.Instance.returnToEditorButton.Visible = true;
    }

    void OnBackButtonPressed()
    {
        GameManager.Instance.ReturnToLevelsMenu();

        //other editor exitng stuff can be handled here
    }
}
