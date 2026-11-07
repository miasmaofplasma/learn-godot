using System;
using System.Collections.Generic;
using Godot;
using TootieFarm.Items;

namespace TootieFarm.Inventory;

public partial class InventoryManager : Node
{
    public Dictionary<ushort, InventoryItem> Items { get; set; } =
        new Dictionary<ushort, InventoryItem>();

    public ushort SelectedSlot { get; set; } = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Items.Add(1, new InventoryItem { Item = ItemRepository._toolhoe });
        Items.Add(2, new InventoryItem { Item = ItemRepository._toolWateringCan });
        Items.Add(3, new InventoryItem { Item = ItemRepository._toolSythe });
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        GD.Print(inputEvent.AsText());
        for (ushort i = 1; i <= 10; i++)
        {
            string actionName = $"inv_{i}";

            // 2. Check if this specific action was just pressed
            if (Input.IsActionJustPressed(actionName))
            {
                SelectedSlot = i;
                EmitSignal(SignalName.SelectedItemChanged, SelectedSlot, GetSelectedItem()?.Item!);
            }
        }
    }

    [Signal]
    public delegate void SelectedItemChangedEventHandler(ushort slot, ItemData? item);

    public InventoryItem? GetSelectedItem()
    {
        return Items.TryGetValue(SelectedSlot, out var item) ? item : null;
    }
}
