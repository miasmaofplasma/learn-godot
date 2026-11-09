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
public record struct UseToolResult(bool Success, Items.CropItem? HarvestedCrops);
internal record Crop(CropData CropData);
public partial class FarmManager : Node
{
    [Export]
    public OutsideMap _outsideMap { get; set; } = null!;
    private Dictionary<Vector2I, Crop> _crops = new();
    [Export]
    private Timemanger _timeManager = null!;
    public override void _Ready()
    {
        FatalException.ThrowIfNull(_outsideMap, nameof(OutsideMap), this);
        _timeManager?.DayUpdated += _ => IncrementDay();
    }
    public CanUseItemEnum CanUseItem(Vector2I cell, Items.ItemData item)
    {
        return item switch
        {
            Items.ToolItem tool => CanUseTool(cell, tool),
            Items.SeedItem seed => CanPlantSeed(cell, seed),
            _ => IsTileUsable(cell)
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
        if (!CanPlantSeed(cell, seed).Equals(CanUseItemEnum.Yes))
        {
            return false;
        }

        var cropData = (CropData)seed.GrownCrop.Duplicate(deep: true);
        _crops.Add(cell, new Crop(cropData));
        _outsideMap.SetCrop(cell, cropData.CropType, cropData.CropCycles.GetStage());
        return true;
    }
    private CanUseItemEnum CanUseTool(Vector2I cell, Items.ToolItem tool)
    {
        var isTileUsable = IsTileUsable(cell);
        if (isTileUsable != CanUseItemEnum.Yes)
        {
            return isTileUsable;
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
        var isTileUsable = IsTileUsable(cell);
        if (isTileUsable != CanUseItemEnum.Yes)
        {
            return isTileUsable;
        }

        if (_outsideMap.GetFarmSoilState(cell) is SoilState.Cleared or SoilState.FlatDry or SoilState.FlatWet)
        {
            return CanUseItemEnum.No;
        }

        if (_crops.ContainsKey(cell))
        {
            return CanUseItemEnum.No;
        }

        return CanUseItemEnum.Yes;
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
        var foundCrop = _crops.TryGetValue(cell, out Crop? crop);
        if (!foundCrop)
        {
            return new UseToolResult { Success = false };
        }
        if (!crop!.CropData.CropCycles.Finished())
        {
            return new UseToolResult { Success = false };
        }

        _outsideMap.ClearCrop(cell);
        _crops.Remove(cell);
        return new UseToolResult { Success = true, HarvestedCrops = Items.ItemRepository.GetCropItem(crop!.CropData.CropType) };
    }

    private CanUseItemEnum IsTileUsable(Vector2I cell)
    {
        if (!_outsideMap.IsGrass(cell))
        {
            return CanUseItemEnum.NotFarmingTile;
        }

        if (_outsideMap.HasObstacle(cell))
        {
            return CanUseItemEnum.No;
        }

        return CanUseItemEnum.Yes;
    }
    private CanUseItemEnum CanTill(Vector2I cell)
    {
        var isTileUsable = IsTileUsable(cell);
        if (isTileUsable != CanUseItemEnum.Yes)
        {
            return isTileUsable;
        }

        var cellType = _outsideMap.GetFarmSoilState(cell);
        if (cellType is SoilState.Cleared or SoilState.FlatDry or SoilState.FlatWet)
        {
            return CanUseItemEnum.Yes;
        }

        return CanUseItemEnum.No;
    }
    private CanUseItemEnum CanWater(Vector2I cell)
    {
        var isTileUsable = IsTileUsable(cell);
        if (isTileUsable != CanUseItemEnum.Yes)
        {
            return isTileUsable;
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
        var isTileUsable = IsTileUsable(cell);
        if (isTileUsable != CanUseItemEnum.Yes)
        {
            return isTileUsable;
        }
        var foundCrop = _crops.TryGetValue(cell, out Crop? crop);
        if (!foundCrop)
        {
            return CanUseItemEnum.No;
        }
        if (!crop!.CropData.CropCycles.Finished())
        {
            return CanUseItemEnum.No;
        }

        return CanUseItemEnum.Yes;
    }

    public void IncrementDay()
    {
        foreach (var (cell, crop) in _crops)
        {
            crop.CropData.CropCycles.IncrementDay();
            _outsideMap.SetCrop(cell, crop.CropData.CropType, crop.CropData.CropCycles.GetStage());
        }
    }
}
