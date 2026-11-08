using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TootieFarm.Inventory.Ui;

public partial class InventoryUI : CanvasLayer
{
    [Export]
    public InventoryManager _inventoryManager = null!;
    private List<Slot> _slots = new();
    private ushort _selectedSlot { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var container = GetNode<HBoxContainer>("%SlotsContainer");
        _slots = container.GetChildren().OfType<Slot>().ToList();

        _inventoryManager.SelectedItemChanged += OnSelectedItemChanged;
        _inventoryManager.InventoryChanged += OnInventoryChanged;

        RenderSlots();
    }
    public void OnSelectedItemChanged(ushort slot)
    {
        _selectedSlot = slot;
        RenderSlots();
    }

    public void OnInventoryChanged()
    {
        RenderSlots();
    }

    private void RenderSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].Render(_inventoryManager.Items[i], _selectedSlot == i);
        }
    }

}
