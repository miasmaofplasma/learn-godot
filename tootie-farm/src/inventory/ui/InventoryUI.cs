using System;
using Godot;

namespace TootieFarm.Inventory.Ui;

public partial class InventoryUI : CanvasLayer
{
    [Export]
    public InventoryManager _inventoryManager = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
