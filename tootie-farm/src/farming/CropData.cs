using Godot;

namespace TootieFarm.Farming;

[GlobalClass]
public partial class CropData : Resource
{
    [Export] public CropType CropType { get; set; }
    [Export] public int SellPrice { get; set; } = 25;
    [Export] public CropCycles CropCycles { get; set; } = null!;
}
