using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Line
{
    private static Line _instance = null;
    private Line() { }

    public static Line Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Line();
            }
            return _instance;
        }
    }

    private bool IsInBounds(Vector2Int position)
    {
        return (position.y < Grid.HEIGHT && (position.x >= 0 && position.x < Grid.WIDTH));
    }

    public List<Vector2Int> Get(Vector2Int start, Vector2Int direction)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.Add(start);

        while(IsInBounds(positions.Last()))
        {
            Vector2Int pivot = positions.Last();
            positions.Add(Neighbour(pivot, direction));
        }

        positions.RemoveAt(positions.Count - 1);

        return positions;
    }

    private Vector2Int Neighbour(Vector2Int pivot, Vector2Int direction)
    {
        if (direction.y != 0) CheckOddRowOffset(ref pivot, ref direction);
        Vector2Int neighbour = pivot + direction;
        return neighbour;
    }

    private void CheckOddRowOffset(ref Vector2Int pivot, ref Vector2Int direction)
    {
        bool oddRow = Math.Abs((pivot.y + direction.y) % 2) == 1;
        if (oddRow && direction.x > 0) direction += new Vector2Int(-1, 0);
        if ((!oddRow) && direction.x < 0) direction += new Vector2Int(+1, 0);
    }

    #region Directions

    public Vector2Int LeftUp
    {
        get
        {
            return new Vector2Int(-1, +1);
        }
    }

    public Vector2Int RightUp
    {
        get
        {
            return new Vector2Int(+1, +1);
        }
    }

    #endregion
}
