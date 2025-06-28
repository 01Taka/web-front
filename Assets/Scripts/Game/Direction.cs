using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionExtensions
{
    public static Vector2 ToVector2(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Vector2.up;
            case Direction.Down: return Vector2.down;
            case Direction.Left: return Vector2.left;
            case Direction.Right: return Vector2.right;
            default: return Vector2.up;
        }
    }
}
