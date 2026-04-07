using Godot;
using System;

public partial class BlockSelectButton : Button
{
	[Export] TileId tileId;

	public override void _Ready()
	{
		Pressed += OnPressed;
	}

    private void OnPressed()
    {
        UIManager.Instance.editor.SelectTile(tileId);
    }
}
