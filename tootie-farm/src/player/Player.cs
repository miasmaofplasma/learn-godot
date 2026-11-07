using System;
using Godot;
using TootieFarm.Farming;
using TootieFarm.Inventory;
using TootieFarm.Items;
using TootieFarm.Player.Model;

namespace TootieFarm.Player;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 120.0f;

    [Export]
    public FarmManager farmManager { get; set; } = null!;

    [Export]
    public InventoryManager _inventory { get; set; } = null!;
    public ItemData? _selectedItem { get; set; } = null;
    private PlayerAnimations _anim = null!;
    private TileSelector _tileSelector = null!;
    private FacingDirection _facingDirection = FacingDirection.Down;
    private float _tileSelectorPosOffset = 16f;

    private Items.ToolItem _hoeTemporary = GD.Load<ToolItem>("res://src/items/tools/hoe.tres");

    public override void _Ready()
    {
        _anim = GetNode<PlayerAnimations>("AnimatedSprite2D");
        _tileSelector = GetNode<TileSelector>("TileSelector");
        FatalException.ThrowIfNull(farmManager, nameof(FarmManager), this);
        FatalException.ThrowIfNull(_tileSelector, nameof(TileSelector), this);
        FatalException.ThrowIfNull(_anim, nameof(PlayerAnimations), this);
        FatalException.ThrowIfNull(_inventory, nameof(InventoryManager), this);

        _inventory.SelectedItemChanged += OnSelectedItemChanged;
        _selectedItem = _inventory.GetSelectedItem()?.Item;

        _anim.PlayIdle(_facingDirection);
        UpdateTileSelector();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        Velocity = inputDirection * Speed;
        MoveAndSlide();
        UpdateFacingDirection(inputDirection);
        UpdateAnimation(inputDirection);
        if (DirectionUtils.IsMoving(inputDirection))
        {
            UpdateTileSelector();
        }
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("use_tool"))
        {
            UseEquippedTool();
        }
    }

    private void OnSelectedItemChanged(ushort slot, ItemData? item)
    {
        _selectedItem = item;
        UpdateTileSelector();
    }

    private void UseEquippedTool()
    {
        if (_selectedItem == null || _selectedItem is not ToolItem)
        {
            return;
        }

        UseToolResult result = farmManager.UseTool(
            _tileSelector.CurrentCell,
            (ToolItem)_selectedItem
        );
        UpdateTileSelector();
        GD.Print(
            result.Success
                ? $"Used {_hoeTemporary.DisplayName} on {_tileSelector.CurrentCell}"
                : $"Can't use {_hoeTemporary.DisplayName} on {_tileSelector.CurrentCell}"
        );
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (DirectionUtils.IsMoving(direction))
        {
            _anim.PlayWalk(_facingDirection);
        }
        else
        {
            _anim.PlayIdle(_facingDirection);
        }
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if (DirectionUtils.IsMoving(direction))
        {
            _facingDirection = DirectionUtils.GetFacingDirection(direction);
        }
    }

    private void UpdateTileSelector()
    {
        _tileSelector.TargetPosition =
            GlobalPosition
            + DirectionUtils.CalculateOffsetInDirection(_facingDirection, _tileSelectorPosOffset);

        var currentItem = _inventory.GetSelectedItem();
        if (currentItem == null)
        {
            _tileSelector.Visible = false;
            return;
        }

        var result = farmManager.CanUseItem(_tileSelector.CurrentCell, currentItem.Item);
        var color = result switch
        {
            CanUseItemEnum.Yes => Colors.Yellow,
            CanUseItemEnum.No => Colors.Red,
            CanUseItemEnum.NotFarmingTile => Colors.Yellow,
            _ => throw new NotImplementedException(),
        };

        if (result is CanUseItemEnum.Yes or CanUseItemEnum.No)
        {
            _tileSelector.Visible = true;
        }
        else
        {
            _tileSelector.Visible = false;
        }

        _tileSelector.BorderColor = color;
    }
}
