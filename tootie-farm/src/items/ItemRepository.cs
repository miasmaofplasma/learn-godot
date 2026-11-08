using Godot;

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
}
