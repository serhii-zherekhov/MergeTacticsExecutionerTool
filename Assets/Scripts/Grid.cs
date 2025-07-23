using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public const int WIDTH = 5;
    public const int HEIGHT = 8;

    [SerializeField] private Cell _cellPrefab;
    private Cell[,] _cells;

    private bool _isShowingEnemyUnits = true;
    private List<GameObject> _enemyUnits = new List<GameObject>();

    private List<Vector2Int> _area = null;

    private void Start()
    {
        _cells = new Cell[WIDTH, HEIGHT];
        for(int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Cell cell = Instantiate(_cellPrefab);
                cell.transform.position = Hex.GetWorldPosition(x, y) + this.transform.position;
                cell.transform.SetParent(this.transform);
                _cells[x, y] = cell;
            }
        }
    }

    public GameObject SetUnit(GameObject unitPrefab, Vector2Int position)
    {
        GameObject unit = Instantiate(unitPrefab);
        unit.transform.position = Hex.GetWorldPosition(position) + this.transform.position;
        unit.transform.SetParent(this.transform);
        _cells[position.x, position.y].SetUnit(unit);
        return unit;
    }

    public void SetEnemyUnit(GameObject unitPrefab, Vector2Int position)
    {
        float enemyRotation = 180f;

        GameObject unit = SetUnit(unitPrefab, position);
        unit.transform.RotateAround(unit.transform.position, Vector3.up, enemyRotation);
        unit.SetActive(_isShowingEnemyUnits);
        _enemyUnits.Add(unit);
    }

    public bool HasUnit(Vector2Int position)
    {
        return _cells[position.x, position.y].HasUnit();
    }

    public void ShowUnitAffectedArea(Vector2Int position)
    {
        HideUnitAffectedArea();

        List<Vector2Int> leftUpLine = Line.Instance.Get(position, Line.Instance.LeftUp);
        List<Vector2Int> rightUpLine = Line.Instance.Get(position, Line.Instance.RightUp);

        _area = new List<Vector2Int>();
        _area.AddRange(leftUpLine);
        _area.AddRange(rightUpLine);

        _area.RemoveAt(0);

        foreach(Vector2Int areaPosition in _area)
        {
            _cells[areaPosition.x, areaPosition.y].Select();
        }
    }

    public void HideUnitAffectedArea()
    {
        if (_area == null) return;

        foreach (Vector2Int areaPosition in _area)
        {
            _cells[areaPosition.x, areaPosition.y].Unselect();
        }
        _area.Clear();
        _area = null;
    }

    public void DeleteUnit(Vector2Int position)
    {
        if (!HasUnit(position)) return;

        _cells[position.x, position.y].DeleteUnit();
    }

    public void ShowActiveCells()
    {
        int maxY = HEIGHT / 2;

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                _cells[x, y].IndicateFade();
            }
        }
    }

    public void ToggleShowEnemyUnits()
    {
        _isShowingEnemyUnits = !_isShowingEnemyUnits;

        foreach(GameObject unit in _enemyUnits)
        {
            unit.SetActive(_isShowingEnemyUnits);
        }
    }
}
