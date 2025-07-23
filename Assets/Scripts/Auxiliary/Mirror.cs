using System.Collections.Generic;
using UnityEngine;

public class Mirror 
{
    public static Vector2Int ReflectPosition(Vector2Int position) 
    {
        Vector2Int mirroredPosition = new Vector2Int(Grid.WIDTH, Grid.HEIGHT) - Vector2Int.one - position;
        return mirroredPosition;
    }
}
