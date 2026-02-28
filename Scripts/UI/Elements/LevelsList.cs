using Godot;
using System;
using System.Collections.Generic;

public partial class LevelsList : ScrollContainer
{
    [Export] PackedScene levelPreview;
    [Export] FlowContainer levelsContainer;

    [Export] LineEdit searchBox;
    [Export] Button searchButton;

    List<LevelPreview> previews = [];
    string searchString;

    public override void _Ready()
    {
        searchBox.TextChanged += OnSearchTextChanged;
        searchButton.Pressed += OnSearchButtonPressed;
    }

    void OnSearchTextChanged(string newText)
    {
        if (newText is not null)
        {
            searchString = newText;
            OnSearchButtonPressed();
        }
        else
        {
            //show all levels
            foreach (var preview in previews)
            {
                preview.Visible = true;
            }
        }
        
        
    }

    void OnSearchButtonPressed()
    {
        if (searchString is null) return;

        foreach (var preview in previews)
        {
            //if level name contains search string (case insensitive), let it show
            if (preview.level.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            {
                preview.Visible = true;
            }
            else preview.Visible = false;
        }
    }

    public void SetLevels(List<Level> levels)
    {
        //clear out previous levels
        ClearLevels();

        //add new levels
        foreach (Level level in levels)
        {
            if (level == null) return;
            LevelPreview newPreview = levelPreview.Instantiate() as LevelPreview;
            newPreview.SetLevel(level);
            levelsContainer.AddChild(newPreview);
            previews.Add(newPreview);
        }
    }

    public void ClearLevels()
    {
        foreach (LevelPreview preview in previews) preview.QueueFree();
        previews.Clear();
    }
}
