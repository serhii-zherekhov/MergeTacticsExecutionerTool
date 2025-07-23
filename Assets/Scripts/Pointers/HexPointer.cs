using System;
using System.Collections.Generic;
using UnityEngine;

public class HexPointer : MonoBehaviour
{
    private const int DISTANCE_BETWEEN_GRIDS = 50;

    public static HexPointer Instance { get; private set; }

    public event EventHandler<Vector2IntEventArgs> OnPositionChanged;

    [SerializeField] private Transform _hexPointer;

    private Vector2Int _position = Vector2Int.zero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _hexPointer = Instantiate(_hexPointer);
        _hexPointer.SetParent(this.transform);
    }

    private void Update()
    {
        Vector3 pointerWorldPosition = Pointer.Instance.GetWorldPosition();
        Vector2Int position = Hex.GetPosition(pointerWorldPosition); 

        ScreenHalf screenHalf = Pointer.Instance.ScreenHalf;
        if (screenHalf == ScreenHalf.Player)
        {
            if (position.x < 0) position.x = 0;
            if (position.x >= Grid.WIDTH) position.x = Grid.WIDTH - 1;
        }

        if (screenHalf == ScreenHalf.Enemy)
        {
            if (position.x < DISTANCE_BETWEEN_GRIDS) position.x = DISTANCE_BETWEEN_GRIDS;
            if (position.x >= Grid.WIDTH + DISTANCE_BETWEEN_GRIDS) position.x = Grid.WIDTH + DISTANCE_BETWEEN_GRIDS - 1;
        }

        if (position.y < 0) position.y = 0;
        if (position.y >= Grid.HEIGHT) position.y = Grid.HEIGHT - 1;


        if (_position != position)
        {
            _position = position;

            _hexPointer.transform.position = Hex.GetWorldPosition(position);

            Vector2IntEventArgs args = new()
            {
                Position = position
            };

            OnPositionChanged?.Invoke(this, args);
        }
    }

    public void ShowCursor(bool isActive)
    {
        _hexPointer.gameObject.SetActive(isActive);
    }

    #region Properties
    public Vector2Int Position
    {
        get
        {
            ScreenHalf screenHalf = Pointer.Instance.ScreenHalf;

            if(screenHalf == ScreenHalf.Enemy)
            {
                return _position - new Vector2Int(DISTANCE_BETWEEN_GRIDS, 0);
            }

            return _position;
        }
    }

    public ScreenHalf ScreenHalf
    {
        get
        {
            return Pointer.Instance.ScreenHalf;
        }
    }
    #endregion
}