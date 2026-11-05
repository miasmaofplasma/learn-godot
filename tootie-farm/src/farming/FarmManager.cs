using System;
using System.Collections.Generic;
using Godot;

namespace TootieFarm.Farming;

public enum CanUseItemEnum
{
    Yes,
    No,
    NotFarmingTile,
}

public record struct UseToolResult(bool Success, List<Items.CropItem> HarvestedCrops);
public partial class FarmManager : Node
{
    [Export]
    public OutsideMap _outsideMap { get; set; } = null!;
    public override void _Ready()
    {
        FatalException.ThrowIfNull(_outsideMap, nameof(OutsideMap), this);
    }
    public CanUseItemEnum CanUseItem(Vector2I cell, Items.ItemData item)
    {
        return item switch
        {
            Items.ToolItem tool => CanUseTool(cell, tool),
            Items.SeedItem seed => CanPlantSeed(cell, seed),
            _ => IsFarmingTile(cell) ? CanUseItemEnum.NotFarmingTile : CanUseItemEnum.No,
        };
    }
    public UseToolResult UseTool(Vector2I cell, Items.ToolItem tool)
    {
        return tool.Action switch
        {
            Items.ActionType.Till => UseTillAction(cell),
            Items.ActionType.Water => UseWaterAction(cell),
            Items.ActionType.Harvest => UseHarvestAction(cell),
            _ => throw new NotImplementedException(),
        };
    }
    public bool PlantSeed(Vector2I cell, Items.SeedItem seed)
    {
        return true;
    }
    private bool IsFarmingTile(Vector2I cell)
    {
        return _outsideMap.IsGrass(cell) && !_outsideMap.HasObstacle(cell);
    }
    private CanUseItemEnum CanUseTool(Vector2I cell, Items.ToolItem tool)
    {
        if (!IsFarmingTile(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }

        return tool.Action switch
        {
            Items.ActionType.Till => CanTill(cell),
            Items.ActionType.Harvest => CanHarvest(cell),
            Items.ActionType.Water => CanWater(cell),
            _ => CanUseItemEnum.No,
        };
    }
    private CanUseItemEnum CanPlantSeed(Vector2I cell, Items.SeedItem tool)
    {
        if (!IsFarmingTile(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }
        return CanUseItemEnum.No;
    }

    private UseToolResult UseTillAction(Vector2I cell)
    {
        GD.Print("Using Till Action");
        if (!CanTill(cell).Equals(CanUseItemEnum.Yes))
        {
            return new UseToolResult { Success = false };
        }

        SoilState soil = _outsideMap.GetFarmSoilState(cell);
        var transformSoilType = soil switch
        {
            SoilState.Cleared => SoilState.FlatDry,
            SoilState.FlatDry => SoilState.PlowedDry,
            SoilState.FlatWet => SoilState.PlowedWet,
            var c => c,
        };
        _outsideMap.SetSoil(cell, transformSoilType);

        return new UseToolResult { Success = true };
    }
    private UseToolResult UseWaterAction(Vector2I cell)
    {
        GD.Print("Using Water Action");
        if (!CanWater(cell).Equals(CanUseItemEnum.Yes))
        {
            return new UseToolResult { Success = false };
        }

        SoilState soil = _outsideMap.GetFarmSoilState(cell);
        var transformSoilType = soil switch
        {
            SoilState.FlatDry => SoilState.FlatWet,
            SoilState.PlowedDry => SoilState.PlowedWet,
            var c => c,
        };

        _outsideMap.SetSoil(cell, transformSoilType);
        return new UseToolResult { Success = true };
    }

    private UseToolResult UseHarvestAction(Vector2I cell)
    {
        throw new NotImplementedException();
        // if (!CanHarvest(cell).Equals(CanUseItemEnum.Yes))
        // {
        //     return new UseToolResult { Success = false };
        // }
        //
        // return new UseToolResult { Success = true };
    }

    private CanUseItemEnum CanTill(Vector2I cell)
    {
        if (!IsFarmingTile(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }

        var cellType = _outsideMap.GetFarmSoilState(cell);
        if (cellType is SoilState.Cleared or SoilState.FlatWet or SoilState.FlatDry)
        {
            return CanUseItemEnum.Yes;
        }

        return CanUseItemEnum.No;
    }
    private CanUseItemEnum CanWater(Vector2I cell)
    {
        if (!IsFarmingTile(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }
        var cellType = _outsideMap.GetFarmSoilState(cell);
        if (cellType is SoilState.FlatDry or SoilState.PlowedDry)
        {
            return CanUseItemEnum.Yes;
        }

        return CanUseItemEnum.No;
    }
    private CanUseItemEnum CanHarvest(Vector2I cell)
    {
        if (!IsFarmingTile(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }
        throw new NotImplementedException();

    }

}
