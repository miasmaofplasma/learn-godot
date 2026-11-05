using Godot;
using TootieFarm.Player.Model;

namespace TootieFarm.Player;

public partial class PlayerAnimations : AnimatedSprite2D
{
    // Centralized animation name constants
    private const string AnimIdleDown = "idle_down";
    private const string AnimIdleUp = "idle_up";
    private const string AnimIdleLeft = "idle_left";
    private const string AnimIdleRight = "idle_right";

    private const string AnimWalkDown = "walk_down";
    private const string AnimWalkUp = "walk_up";
    private const string AnimWalkLeft = "walk_left";
    private const string AnimWalkRight = "walk_right";

    public void PlayIdle(FacingDirection direction)
    {
        string anim = direction switch
        {
            FacingDirection.Down => AnimIdleDown,
            FacingDirection.Up => AnimIdleUp,
            FacingDirection.Left => AnimIdleLeft,
            FacingDirection.Right => AnimIdleRight,
            _ => AnimIdleDown
        };

        Play(anim);
    }

    public void PlayWalk(FacingDirection direction)
    {
        string anim = direction switch
        {
            FacingDirection.Down => AnimWalkDown,
            FacingDirection.Up => AnimWalkUp,
            FacingDirection.Left => AnimWalkLeft,
            FacingDirection.Right => AnimWalkRight,
            _ => AnimWalkDown
        };

        Play(anim);
    }
}
