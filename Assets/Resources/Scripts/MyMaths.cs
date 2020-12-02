using EpPathFinding.cs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMaths 
{
    public static Vector2 GridPosToVector2(GridPos position)
    {
        return new Vector2(position.x, position.y);
    }

    public static GridPos Vector2ToGridPos(Vector2Int position)
    {
        return new GridPos(position.x, position.y);
    }

    public static float GetAngle(Vector2 targetPosition)
    {
        return Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
    }

    public static float GetAngle(Vector2 startPosition, Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - startPosition;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
