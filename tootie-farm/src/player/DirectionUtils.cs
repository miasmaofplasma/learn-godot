using Godot;
using TootieFarm.Player.Model;

namespace TootieFarm.Player;

public static class DirectionUtils
{
    public static bool IsMoving(Vector2 direction)
    {
        return direction != Vector2.Zero;
    }

    public static Vector2 CalculateOffsetInDirection(FacingDirection direction, float offset)
    {
        switch (direction)
        {
            case FacingDirection.Left: return new Vector2(-offset, 0);
            case FacingDirection.Right: return new Vector2(offset, 0);
            case FacingDirection.Up: return new Vector2(0, -offset);
            case FacingDirection.Down: return new Vector2(0, offset);
        }

        return new Vector2(0, 0);
    }
    public static FacingDirection GetFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            return direction.X > 0 ? FacingDirection.Right : FacingDirection.Left;
        }
        else
        {
            return direction.Y > 0 ? FacingDirection.Down : FacingDirection.Up;
        }
    }
}
