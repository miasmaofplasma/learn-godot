using Godot;
using System;

public partial class TileSelector : Node2D
{
    // Called when the node enters the scene tree for the first time.
    [Export]
    public Vector2I TileSize { get; set; } = new Vector2I(32, 32);

    [Export]
    public Color BorderColor { get; set; } = Colors.Yellow;

    public Vector2I CurrentCell
    {
        get; private set
        {
            if (field != value)
            {
                field = value;
                GlobalPosition = new Vector2(value.X * TileSize.X, value.Y * TileSize.Y);
            }
        }
    }
    public Vector2 TargetPosition
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                CurrentCell = _GetCell(value);
            }
        }
    }
    public override void _Draw()
    {
        DrawRect(new Rect2(0, 0, TileSize.X, TileSize.Y), BorderColor, filled: false, width: 2f);
    }
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    private Vector2I _GetCell(Vector2 targetPosition)
    {
        int cellX = Mathf.FloorToInt(targetPosition.X / TileSize.X);
        int cellY = Mathf.FloorToInt(targetPosition.Y / TileSize.Y);
        return new Vector2I(cellX, cellY);
    }
}
