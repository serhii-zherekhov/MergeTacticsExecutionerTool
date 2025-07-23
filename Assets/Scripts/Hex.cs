using System;
using System.Collections.Generic;
using UnityEngine;

public static class Hex
{
    private static class Hexagon 
    {
        private const float WIDTH = 2.0f;
        private const float HEIGHT = 2.3094f;

        private const float HALF = 0.5f;
        private const float QUARTER = 0.25f;

        public static readonly float _width;
        public static readonly float _height;
        public static readonly Vector3 _centerPoint;
        public static readonly Vector3 _upperRightCorner;
        public static readonly Vector3 _upperLeftCorner;
        public static readonly Vector3 _upperCorner;
        public static readonly Vector3 _lowerRightCorner;
        public static readonly Vector3 _lowerLeftCorner;
        public static readonly Vector3 _lowerCorner;

        static Hexagon() 
        {
            _centerPoint = Vector3.zero;
            _width = WIDTH;
            _height = HEIGHT;

            _upperCorner = _centerPoint + new Vector3(0, 0, 1) * (_height * HALF);
            _lowerCorner = _centerPoint + new Vector3(0, 0, -1) * (_height * HALF);

            _upperRightCorner = _centerPoint + new Vector3(1, 0, 0) * (_width * HALF) + new Vector3(0, 0, 1) * (_height * QUARTER);
            _upperLeftCorner = _centerPoint + new Vector3(-1, 0, 0) * (_width * HALF) + new Vector3(0, 0, 1) * (_height * QUARTER);
            _lowerRightCorner = _centerPoint + new Vector3(1, 0, 0) * (_width * HALF) + new Vector3(0, 0, -1) * (_height * QUARTER);
            _lowerLeftCorner = _centerPoint + new Vector3(-1, 0, 0) * (_width * HALF) + new Vector3(0, 0, -1) * (_height * QUARTER);
        }
    }

    private const float NINETY_DEGREES_CLOCKWISE_ROTATION = 90f;
    private const float NINETY_DEGREES_COUNTERCLOCKWISE_ROTATION = -90f;

    private const float HEX_VERTICAL_OFFSET = 0.75f;

    private static List<Vector2Int> _neighboursOdd = new List<Vector2Int>
        {
            new Vector2Int(-1, 0),
            new Vector2Int(+1, 0),
                           
            new Vector2Int(+1, +1),
            new Vector2Int(+0, +1),
                           
            new Vector2Int(+1, -1),
            new Vector2Int(+0, -1)
        };

    private static List<Vector2Int> _neighboursEven = new List<Vector2Int>
        {
            new Vector2Int(-1, 0),
            new Vector2Int(+1, 0),
                      
            new Vector2Int(-1, +1),
            new Vector2Int(+0, +1),
                      
            new Vector2Int(-1, -1),
            new Vector2Int(+0, -1)
        };

    public static Vector2Int GetPosition(Vector3 worldPosition) 
    {
        int roughX = Mathf.RoundToInt(worldPosition.x / Hex.Hexagon._width);
        int roughY = Mathf.RoundToInt(worldPosition.z / Hex.Hexagon._height / HEX_VERTICAL_OFFSET);

        Vector2Int roughXY = new Vector2Int(roughX, roughY);
        Vector2Int closestPosition = roughXY;

        bool oddRow = roughY % 2 == 1;
        List<Vector2Int> neighbours = (oddRow) ? _neighboursOdd : _neighboursEven;

        foreach (Vector2Int neighbour in neighbours) 
        {
            Vector2Int neighbourPosition = roughXY + neighbour;

            if (DistanceBetweenWorldPositions(worldPosition, GetWorldPosition(neighbourPosition)) <
                DistanceBetweenWorldPositions(worldPosition, GetWorldPosition(closestPosition))) 
            {
                closestPosition = neighbourPosition;
            }
        }

        return closestPosition;
    }

    public static Vector3 GetWorldPosition(float x, float y) 
    {
        return new Vector3(x * Hex.Hexagon._width + Math.Abs(y % 2), 0, y * Hex.Hexagon._height * HEX_VERTICAL_OFFSET);
    }

    public static Vector3 GetWorldPosition(Vector2Int position) 
    {
        return GetWorldPosition(position.x, position.y);
    }

    public static float DistanceBetweenWorldPositions(Vector3 worldPosition1, Vector3 worldPosition2) 
    {
        return Vector3.Distance(worldPosition1, worldPosition2);
    }

    public static bool IsInsideHex(Vector3 worldPosition, Vector2Int hexPosition) 
    {
        return IsInsideHex(worldPosition, GetWorldPosition(hexPosition));
    }

    public static bool IsInsideHex(Vector3 worldPosition, Vector3 hexWorldPosition) 
    {
        worldPosition -= hexWorldPosition;

        if (worldPosition.x < Hexagon._lowerRightCorner.x &&
             worldPosition.x > Hexagon._lowerLeftCorner.x) 
        {
            // Inside horizontal bounds

            if (worldPosition.z < Hexagon._upperCorner.z &&
                worldPosition.z > Hexagon._lowerCorner.z) 
            {
                // Inside vertical bounds

                Vector3 dirFromUpperRightCornerToUpperCorner = Hexagon._upperCorner - Hexagon._upperRightCorner;
                Vector3 dirFromUpperRightCornerToUpperCornerRotatedNinetyDegreesCounterclockwise = Quaternion.Euler(0f, NINETY_DEGREES_COUNTERCLOCKWISE_ROTATION, 0f) * dirFromUpperRightCornerToUpperCorner;
                Vector3 dirFromUpperRightCornerToPosition = worldPosition - Hexagon._upperRightCorner;
                float dotUpperRightCorner = Vector3.Dot(dirFromUpperRightCornerToUpperCornerRotatedNinetyDegreesCounterclockwise.normalized, dirFromUpperRightCornerToPosition.normalized);

                Vector3 dirFromUpperLeftCornerToUpperCorner = Hexagon._upperCorner - Hexagon._upperLeftCorner;
                Vector3 dirFromUpperLeftCornerToUpperCornerRotatedNinetyDegreesClockwise = Quaternion.Euler(0f, NINETY_DEGREES_CLOCKWISE_ROTATION, 0f) * dirFromUpperLeftCornerToUpperCorner;
                Vector3 dirFromUpperLeftCornerToPosition = worldPosition - Hexagon._upperLeftCorner;
                float dotUpperLeftCorner = Vector3.Dot(dirFromUpperLeftCornerToUpperCornerRotatedNinetyDegreesClockwise.normalized, dirFromUpperLeftCornerToPosition.normalized);

                Vector3 dirFromLowerRightCornerToLowerCorner = Hexagon._lowerCorner - Hexagon._lowerRightCorner;
                Vector3 dirFromLowerRightCornerToLowerCornerRotatedNinetyDegreesClockwise = Quaternion.Euler(0f, NINETY_DEGREES_CLOCKWISE_ROTATION, 0f) * dirFromLowerRightCornerToLowerCorner;
                Vector3 dirFromLowerRightCornerToPosition = worldPosition - Hexagon._lowerRightCorner;
                float dotLowerRightCorner = Vector3.Dot(dirFromLowerRightCornerToLowerCornerRotatedNinetyDegreesClockwise.normalized, dirFromLowerRightCornerToPosition.normalized);

                Vector3 dirFromLowerLeftCornerToLowerCorner = Hexagon._lowerCorner - Hexagon._lowerLeftCorner;
                Vector3 dirFromLowerLeftCornerToLowerCornerRotatedNinetyDegreesCounterclockwise = Quaternion.Euler(0f, NINETY_DEGREES_COUNTERCLOCKWISE_ROTATION, 0f) * dirFromLowerLeftCornerToLowerCorner;
                Vector3 dirFromLowerLeftCornerToPosition = worldPosition - Hexagon._lowerLeftCorner;
                float dotLowerLeftCorner = Vector3.Dot(dirFromLowerLeftCornerToLowerCornerRotatedNinetyDegreesCounterclockwise.normalized, dirFromLowerLeftCornerToPosition.normalized);

                if (dotUpperRightCorner > 0 && dotUpperLeftCorner > 0 && dotLowerRightCorner > 0 && dotLowerLeftCorner > 0) 
                {
                    // Inside diagonal borders
                    return true;
                }
            }
        }
        return false;
    }

    #region Properties

    public static Vector3 UpperRightCorner
    {
        get
        {
            return Hexagon._upperRightCorner;
        }
    }

    public static Vector3 UpperLeftCorner
    {
        get
        {
            return Hexagon._upperLeftCorner;
        }
    }

    public static Vector3 UpperCorner
    {
        get
        {
            return Hexagon._upperCorner;
        }
    }

    public static Vector3 LowerRightCorner
    {
        get
        {
            return Hexagon._lowerRightCorner;
        }
    }

    public static Vector3 LowerLeftCorner
    {
        get
        {
            return Hexagon._lowerLeftCorner;
        }
    }
    
    public static Vector3 LowerCorner
    {
        get
        {
            return Hexagon._lowerCorner;
        }
    }

    public static Vector3 HalfUpperRightCorner
    {
        get
        {
            return Hexagon._upperRightCorner / 2f;
        }
    }

    public static Vector3 HalfUpperLeftCorner
    {
        get
        {
            return Hexagon._upperLeftCorner / 2f;
        }
    }

    public static Vector3 HalfUpperCorner
    {
        get
        {
            return Hexagon._upperCorner / 2f;
        }
    }

    public static Vector3 HalfLowerRightCorner
    {
        get
        {
            return Hexagon._lowerRightCorner / 2f;
        }
    }

    public static Vector3 HalfLowerLeftCorner
    {
        get
        {
            return Hexagon._lowerLeftCorner / 2f;
        }
    }

    public static Vector3 HalfLowerCorner
    {
        get
        {
            return Hexagon._lowerCorner / 2f;
        }
    }

    #endregion
}
