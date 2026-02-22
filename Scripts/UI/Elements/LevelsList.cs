using Godot;
using System;
using System.Collections.Generic;

public partial class LevelsList : ScrollContainer
{
    [Export] PackedScene levelPreview;
    [Export] FlowContainer levelsContainer;

	List<LevelPreview> previews = [];

    public void SetLevels(List<Level> levels)
    {
        //clear out previous levels
        foreach (LevelPreview preview in previews) preview.QueueFree();
        previews.Clear();

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
}
