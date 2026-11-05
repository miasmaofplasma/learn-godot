using System.Collections.Generic;
using Godot;
using TootieFarm.Farming;

namespace TootieFarm.Tests;

public partial class TestOutsideMap : OutsideMap
{
    public HashSet<Vector2I> GrassCells { get; } = new();
    public HashSet<Vector2I> ObstacleCells { get; } = new();
    public Dictionary<Vector2I, SoilState> SoilStates { get; } = new();
    public Dictionary<Vector2I, (CropType Crop, int Stage)> PlacedCrops { get; } = new();

    public override bool HasObstacle(Vector2I cell) => ObstacleCells.Contains(cell);
    public override bool IsGrass(Vector2I cell) => GrassCells.Contains(cell);
    public override SoilState GetFarmSoilState(Vector2I cell) =>
        SoilStates.GetValueOrDefault(cell, SoilState.Cleared);

    public override void SetSoil(Vector2I cell, SoilState state)
    {
        if (state == SoilState.Cleared)
            SoilStates.Remove(cell);
        else
            SoilStates[cell] = state;
    }

    public override void SetCrop(Vector2I cell, CropType crop, int stage)
    {
        PlacedCrops[cell] = (crop, stage);
    }

    public override void ClearCrop(Vector2I cell)
    {
        PlacedCrops.Remove(cell);
    }
}
