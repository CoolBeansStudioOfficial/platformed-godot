using Godot;
using System;
using System.Collections.Generic;

public partial class EditorSelection : Node
{
}

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

    public void Fill(TileId id)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            TileInfo tile = tiles[i];
            tile.id = id;
            tiles[i] = tile;
        }
    }

    //only pass in left or right
    public void Rotate(TileRotation direction)
    {
        if (direction == TileRotation.Left)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                int rotation = (int)tiles[i].rotation;

                if (rotation < 3) rotation++;
                else rotation = 0;

                TileInfo tile = tiles[i];
                tile.rotation = (TileRotation)rotation;
                tiles[i] = tile;
                GD.Print($"rotated left: {tiles[i].rotation}");
            }
        }
        else if (direction == TileRotation.Right)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                int rotation = (int)tiles[i].rotation;

                if (rotation > 0) rotation--;
                else rotation = 3;

                TileInfo tile = tiles[i];
                tile.rotation = (TileRotation)rotation;
                tiles[i] = tile;
                GD.Print($"rotated right: {tiles[i].rotation}");
            }
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

    public Vector2I GetCorner()
    {
        return new(Mathf.Min(start.X, end.X), Mathf.Min(start.Y, end.Y));
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
