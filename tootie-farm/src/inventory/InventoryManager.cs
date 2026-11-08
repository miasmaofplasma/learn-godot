using System.Collections.Generic;
using Godot;
using TootieFarm.Items;

namespace TootieFarm.Inventory;

public partial class InventoryManager : Node
{
    public const int MaxSlots = 10;
    public List<InventoryItem?> Items { get; private set; } = new(new InventoryItem?[MaxSlots]);
    public ushort SelectedSlot
    {
        get; set
        {
            if (field != value && value < MaxSlots)
            {
                field = value;
                EmitSignal(SignalName.SelectedItemChanged, value);
            }
        }
    } = ushort.MaxValue;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Items[0] = new InventoryItem { Item = ItemRepository._toolhoe };
        Items[1] = new InventoryItem { Item = ItemRepository._toolWateringCan };
        Items[2] = new InventoryItem { Item = ItemRepository._toolBasket };
        Items[3] = new InventoryItem { Item = ItemRepository._seedsRadish, Count = 5 };
        Items[4] = new InventoryItem { Item = ItemRepository._seedsTomato, Count = 15 };
        Items[5] = new InventoryItem { Item = ItemRepository._seedsCarrot, Count = 15 };
        Items[6] = new InventoryItem { Item = ItemRepository._seedsCorn, Count = 15 };
        Items[7] = new InventoryItem { Item = ItemRepository._seedsPumpkin, Count = 15 };

        EmitSignal(SignalName.InventoryChanged);
        SelectedSlot = 0;
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
                SelectedSlot = (ushort)(i - 1);
            }
        }
    }
    public InventoryItem? GetSelectedItem()
    {
        return SelectedSlot < Items.Count ? Items[SelectedSlot] : null;
    }

    public void AddItem(ItemData item)
    {
        var index = Items.FindIndex(invItem => invItem?.Item == item);
        if (index != -1)
        {
            var foundItem = Items[index]!;
            foundItem.Count++;
        }
        else
        {
            var emptyIndex = Items.FindIndex(invItem => invItem == null);
            if (emptyIndex != -1)
            {
                Items[emptyIndex] = new InventoryItem { Item = item, Count = 1 };
            }
        }
        EmitSignal(SignalName.InventoryChanged);
    }

    public void DecrementItem(ItemData item)
    {
        var index = Items.FindIndex(invItem => invItem?.Item == item);
        if (index != -1)
        {
            var foundItem = Items[index]!;
            foundItem.Count--;
            if (foundItem.Count == 0)
            {
                Items[index] = null;
            }
            EmitSignal(SignalName.InventoryChanged);
        }
    }

    [Signal]
    public delegate void SelectedItemChangedEventHandler(ushort slot);

    [Signal]
    public delegate void InventoryChangedEventHandler();
}
