using System;
using Godot;
using TootieFarm.Farming;

public enum SoilState
{
    FlatDry,
    PlowedDry,
    FlatWet,
    PlowedWet,
    Cleared,
}

public partial class OutsideMap : Node2D
{
    public TileMapLayer GroundLayer { get; private set; } = null!;
    public TileMapLayer FarmLayer { get; private set; } = null!;
    public TileMapLayer CropsLayer { get; private set; } = null!;
    public TileMapLayer ObjectsLayer { get; private set; } = null!;

    private static readonly Vector2I TileFlatDry = new(0, 1);
    private static readonly Vector2I TilePlowedDry = new(2, 1);
    private static readonly Vector2I TileFlatWet = new(0, 2);
    private static readonly Vector2I TilePlowedWet = new(2, 2);

    public override void _Ready()
    {
        GroundLayer = GetNode<TileMapLayer>("GroundLayer");
        FarmLayer = GetNode<TileMapLayer>("FarmLayer");
        CropsLayer = GetNode<TileMapLayer>("CropsLayer");
        ObjectsLayer = GetNode<TileMapLayer>("ObjectsLayer");
    }

    public virtual bool HasObstacle(Vector2I cell)
    {
        TileData objectData = ObjectsLayer.GetCellTileData(cell);
        if (objectData == null)
        {
            return false;
        }
        return objectData.GetCustomData("blocks_farming").AsBool();
    }

    public virtual SoilState GetFarmSoilState(Vector2I cell)
    {
        Vector2I cellType = FarmLayer.GetCellAtlasCoords(cell);
        return cellType switch
        {
            var c when c == TileFlatDry => SoilState.FlatDry,
            var c when c == TileFlatWet => SoilState.FlatWet,
            var c when c == TilePlowedDry => SoilState.PlowedDry,
            var c when c == TilePlowedWet => SoilState.PlowedWet,
            _ => SoilState.Cleared,
        };
    }

    public virtual bool IsGrass(Vector2I cell)
    {
        return GroundLayer?.GetCellAtlasCoords(cell).Y == 0;
    }

    public virtual void SetSoil(Vector2I cell, SoilState state)
    {
        switch (state)
        {
            case SoilState.FlatDry:
                FarmLayer?.SetCell(cell, 0, TileFlatDry);
                break;
            case SoilState.PlowedDry:
                FarmLayer?.SetCell(cell, 0, TilePlowedDry);
                break;
            case SoilState.FlatWet:
                FarmLayer?.SetCell(cell, 0, TileFlatWet);
                break;
            case SoilState.PlowedWet:
                FarmLayer?.SetCell(cell, 0, TilePlowedWet);
                break;
            case SoilState.Cleared:
                FarmLayer?.EraseCell(cell);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public virtual void SetCrop(Vector2I cell, CropType crop, int stage)
    {
        int stageCol = Mathf.Clamp(stage, 0, 3);
        int cropRow = (int)crop;
        CropsLayer?.SetCell(cell, 2, new Vector2I(stageCol, cropRow));
    }

    public virtual void ClearCrop(Vector2I cell)
    {
        CropsLayer?.EraseCell(cell);
    }
}
