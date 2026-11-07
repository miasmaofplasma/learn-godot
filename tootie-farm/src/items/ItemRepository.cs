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
}
