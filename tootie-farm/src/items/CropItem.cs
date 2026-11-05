using Godot;
using System;
using TootieFarm.Farming;

namespace TootieFarm.Items;

[GlobalClass]
public partial class CropItem : ItemData
{
    [Export] public CropData CropData { get; set; } = null!;
}
