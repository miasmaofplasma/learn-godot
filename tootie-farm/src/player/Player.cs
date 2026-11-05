using Godot;
using System;
using TootieFarm.Player.Model;
using TootieFarm.Farming;
namespace TootieFarm.Player;

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 120.0f;
    [Export]
    public FarmManager farmManager { get; set; } = null!;
    private PlayerAnimations _anim = null!;
    private TileSelector _tileSelector = null!;
    private FacingDirection _facingDirection = FacingDirection.Down;
    private float _tileSelectorPosOffset = 16f;
    public override void _Ready()
    {
        _anim = GetNode<PlayerAnimations>("AnimatedSprite2D");
        _tileSelector = GetNode<TileSelector>("TileSelector");
        FatalException.ThrowIfNull(farmManager, nameof(FarmManager), this);
        FatalException.ThrowIfNull(_tileSelector, nameof(TileSelector), this);
        FatalException.ThrowIfNull(_anim, nameof(PlayerAnimations), this);

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
        _tileSelector.TargetPosition = GlobalPosition + DirectionUtils.CalculateOffsetInDirection(_facingDirection, _tileSelectorPosOffset);
    }
}
