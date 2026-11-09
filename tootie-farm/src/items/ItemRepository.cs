using System;
using Godot;
using TootieFarm.Farming;

namespace TootieFarm.Items;

public class ItemRepository
{
    public static ToolItem _toolhoe { get; private set; } =
        GD.Load<ToolItem>("res://src/items/tools/hoe.tres");
    public static ToolItem _toolWateringCan { get; private set; } =
        GD.Load<ToolItem>("res://src/items/tools/watering_can.tres");
    public static ToolItem _toolSythe { get; private set; } =
        GD.Load<ToolItem>("res://src/items/tools/sythe.tres");
    public static ToolItem _toolBasket { get; private set; } =
        GD.Load<ToolItem>("res://src/items/tools/basket.tres");
    public static SeedItem _seedsRadish { get; private set; } =
        GD.Load<SeedItem>("res://src/items/seeds/radish.tres");
    public static SeedItem _seedsTomato { get; private set; } =
        GD.Load<SeedItem>("res://src/items/seeds/tomato.tres");
    public static SeedItem _seedsCarrot { get; private set; } =
        GD.Load<SeedItem>("res://src/items/seeds/carrot.tres");
    public static SeedItem _seedsCorn { get; private set; } =
        GD.Load<SeedItem>("res://src/items/seeds/corn.tres");
    public static SeedItem _seedsPumpkin { get; private set; } =
        GD.Load<SeedItem>("res://src/items/seeds/pumpkin.tres");
    public static CropItem _cropRadish { get; private set; } =
        GD.Load<CropItem>("res://src/items/crops/radish.tres");
    public static CropItem _cropTomato { get; private set; } =
        GD.Load<CropItem>("res://src/items/crops/tomato.tres");
    public static CropItem _cropCarrot { get; private set; } =
        GD.Load<CropItem>("res://src/items/crops/carrot.tres");
    public static CropItem _cropCorn { get; private set; } =
        GD.Load<CropItem>("res://src/items/crops/corn.tres");
    public static CropItem _cropPumpkin { get; private set; } =
        GD.Load<CropItem>("res://src/items/crops/pumpkin.tres");

    public static CropItem GetCropItem(CropType cropType) => cropType switch
    {
        CropType.Radish => _cropRadish,
        CropType.Tomato => _cropTomato,
        CropType.Carrot => _cropCarrot,
        CropType.Corn => _cropCorn,
        CropType.Pumpkin => _cropPumpkin,
        _ => throw new ArgumentOutOfRangeException(nameof(cropType), cropType, null)
    };
}
