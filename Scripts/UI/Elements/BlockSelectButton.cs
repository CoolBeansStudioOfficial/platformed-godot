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
		var texture = (StyleBoxTexture)GetThemeStylebox("normal");
        UIManager.Instance.editor.SelectTile(new()
		{
			id = tileId,
			texture = ((StyleBoxTexture)GetThemeStylebox("normal")).Texture,
			region = ((StyleBoxTexture)GetThemeStylebox("normal")).RegionRect,
        });
    }
}
