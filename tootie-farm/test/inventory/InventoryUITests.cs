using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using TootieFarm.Inventory;
using TootieFarm.Inventory.Ui;

namespace TootieFarm.Tests;

[TestSuite]
[RequireGodotRuntime]
public class InventoryUITests
{
    [TestCase]
    public void Ready_HighlightsAlreadySelectedSlot()
    {
        var inventory = new InventoryManager();
        inventory._Ready();
        inventory.SelectedSlot = 3;
        var ui = GD.Load<PackedScene>("res://src/inventory/ui/inventory_ui.tscn").Instantiate<InventoryUI>();
        ui._inventoryManager = inventory;
        ((SceneTree)Engine.GetMainLoop()).Root.AddChild(ui);
        var slots = ui.GetNode<HBoxContainer>("%SlotsContainer").GetChildren();

        try
        {
            AssertBool(((Slot)slots[3]).SelectedBox.Visible).IsTrue();
            AssertBool(((Slot)slots[0]).SelectedBox.Visible).IsFalse();
        }
        finally
        {
            ui.Free();
            inventory.Free();
        }
    }
}
