using Godot;
using System;

namespace TootieFarm.Inventory;

public partial class Slot : PanelContainer
{

    public TextureRect Icon { get; private set; } = null!;
    public Label CountLabel { get; private set; } = null!;
    public ReferenceRect SelectedBox { get; private set; } = null!;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("Icon");
        CountLabel = GetNode<Label>("CountLabel");
        SelectedBox = GetNode<ReferenceRect>("SelectedBox");
    }
    public void Render(InventoryItem? inventoryItem, bool selected)
    {
        SelectedBox.Visible = selected;
        if (inventoryItem == null)
        {
            Icon.Texture = null;
            CountLabel.Text = "";
            return;
        }

        Icon.Texture = inventoryItem.Item.Icon;
        CountLabel.Text = inventoryItem.Item.MaxStack > 1 ? inventoryItem.Count.ToString() : "";
    }
}
