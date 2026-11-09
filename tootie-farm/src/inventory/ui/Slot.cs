using Godot;
using System;

namespace TootieFarm.Inventory;

public partial class Slot : PanelContainer
{

    public TextureRect Icon { get; private set; } = null!;
    public Label CountLabel { get; private set; } = null!;
    public ReferenceRect SelectedBox { get; private set; } = null!;
    private ColorRect _hoverOverlay = null!;
    private Tween? _hoverTween;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("Icon");
        CountLabel = GetNode<Label>("CountLabel");
        SelectedBox = GetNode<ReferenceRect>("SelectedBox");
        _hoverOverlay = GetNode<ColorRect>("HoverOverlay");

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
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


    [Signal]
    public delegate void SlotClickedEventHandler();

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton
                && mouseButton.ButtonIndex == MouseButton.Left
                && mouseButton.Pressed)
        {
            EmitSignal(SignalName.SlotClicked);
            AcceptEvent();
        }
    }
    private void OnMouseEntered()
    {
        FadeHoverTo(.2f);
    }

    private void OnMouseExited()
    {
        FadeHoverTo(0.0f);
    }

    private void FadeHoverTo(float alpha)
    {
        _hoverTween?.Kill();
        _hoverTween = CreateTween();

        _hoverTween.TweenProperty(_hoverOverlay, "modulate:a", alpha, .15);
    }
}
