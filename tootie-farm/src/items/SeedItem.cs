using Godot;
using System;
using TootieFarm.Farming;

namespace TootieFarm.Items;

[GlobalClass]
public partial class SeedItem : ItemData
{
    [Export] public CropData GrownCrop { get; set; } = null!;
}
