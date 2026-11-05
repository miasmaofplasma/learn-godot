using Godot;

namespace TootieFarm.Farming;

[GlobalClass]
public partial class CropData : Resource
{
    [Export] public CropType CropType { get; set; }
    [Export] public int GrowthHoursDuration { get; set; } = 24;
    [Export] public int SellPrcie { get; set; } = 25;
}
