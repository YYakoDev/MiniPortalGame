using UnityEngine;

public static class Directions
{
    public static Vector2Int[] EightDirections = new Vector2Int[8]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up + Vector2Int.right, //up right
        Vector2Int.up + Vector2Int.left, //up left
        Vector2Int.down + Vector2Int.right, //down right
        Vector2Int.down + Vector2Int.left, //down left
    };

    static Vector2 upRight = (Vector2.up + Vector2.right).normalized;
    static Vector2 upLeft = (Vector2.up + Vector2.left).normalized;
    static Vector2 downRight = (Vector2.down + Vector2.right).normalized;
    static Vector2 downLeft = (Vector2.down + Vector2.left).normalized;

    public static Vector2[] NormalizedDirections = new Vector2[8]
    {
        Vector2.up,
        Vector2.down,
        Vector2.right,
        Vector2.left,
        upRight,
        upLeft,
        downRight,
        downLeft
        
    };

}
