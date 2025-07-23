using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Player : MonoBehaviour
{
    private static Vector2Int DISTANCE_BETWEEN_GRIDS = new Vector2Int(50, 0);

    public static Player Instance { get; private set; }

    [SerializeField] private Grid _playerGrid;
    [SerializeField] private Grid _enemyGrid;

    [SerializeField] private GameObject _barbarianPrefab;
    [SerializeField] private GameObject _roguePrefab;

    private Coroutine _showUnitAffectedAreaDelayedCoroutine;



    [SerializeField] private GameObject _helpPanel;


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
        HexPointer.Instance.OnPositionChanged += HandleUnits_OnPositionChanged;
    }

    private void HandleUnits_OnPositionChanged(object sender, Vector2IntEventArgs e)
    {
        if (_showUnitAffectedAreaDelayedCoroutine != null)
        {
            StopCoroutine(_showUnitAffectedAreaDelayedCoroutine);
            _showUnitAffectedAreaDelayedCoroutine = null;
        }

        Vector2Int position = e.Position;
        ScreenHalf screenHalf = HexPointer.Instance.ScreenHalf;

        if (screenHalf == ScreenHalf.Player && position.y < Grid.HEIGHT / 2)
        {
            if (_playerGrid.HasUnit(position))
            {
                _playerGrid.ShowUnitAffectedArea(position);
            }
            else
            {
                _playerGrid.HideUnitAffectedArea();
            }
        }
    }

    private void Update()
    {
        if (Pointer.Instance.IsMouseOverUI) return;

        if (Input.GetMouseButtonDown(Mouse.LeftButton))
        {
            SetUnit();
        }

        if (Input.GetMouseButtonDown(Mouse.RightButton))
        {
            DeleteUnit();
            _playerGrid.HideUnitAffectedArea();
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.F1))
        {
            _helpPanel.SetActive(!_helpPanel.activeSelf);
        }
    }

    private void SetUnit()
    {
        GameObject unitPrefab = _barbarianPrefab;
        Vector2Int position = HexPointer.Instance.Position;
        ScreenHalf screenHalf = HexPointer.Instance.ScreenHalf;

        if (position.y >= Grid.HEIGHT / 2) { ShowActiveCells(); return; }


        if (screenHalf == ScreenHalf.Player)
        {
            if (!_playerGrid.HasUnit(position))
            {
                _playerGrid.SetUnit(_barbarianPrefab, position);

                ShowUnitAffectedAreaDelayed(position);
            }
        }


        if (screenHalf == ScreenHalf.Enemy)
        {
            if (!_enemyGrid.HasUnit(position))
            {
                _playerGrid.SetEnemyUnit(_roguePrefab, Mirror.ReflectPosition(position));
                _enemyGrid.SetUnit(_roguePrefab, position);
            }
        }
    }

    private void DeleteUnit()
    {
        Vector2Int position = HexPointer.Instance.Position;
        ScreenHalf screenHalf = HexPointer.Instance.ScreenHalf;

        if (position.y >= Grid.HEIGHT / 2) { ShowActiveCells(); return; }


        if (screenHalf == ScreenHalf.Player)
        {
            _playerGrid.DeleteUnit(position);
        }


        if (screenHalf == ScreenHalf.Enemy)
        {
            _playerGrid.DeleteUnit(Mirror.ReflectPosition(position));
            _enemyGrid.DeleteUnit(position);
        }
    }

    private void ShowActiveCells()
    {
        ScreenHalf screenHalf = HexPointer.Instance.ScreenHalf;

        if (screenHalf == ScreenHalf.Player)
        {
            _playerGrid.ShowActiveCells();
        }

        if (screenHalf == ScreenHalf.Enemy)
        {
            _enemyGrid.ShowActiveCells();
        }
    }

    public void ToggleShowEnemyUnits()
    {
        _playerGrid.ToggleShowEnemyUnits();
    }

    private void ShowUnitAffectedAreaDelayed(Vector2Int position)
    {
        if (_showUnitAffectedAreaDelayedCoroutine != null)
        {
            StopCoroutine(_showUnitAffectedAreaDelayedCoroutine);
            _showUnitAffectedAreaDelayedCoroutine = null;
        }
        _showUnitAffectedAreaDelayedCoroutine = StartCoroutine(ShowUnitAffectedAreaDelayedCoroutine(position));
    }

    private IEnumerator ShowUnitAffectedAreaDelayedCoroutine(Vector2Int position)
    { 
        yield return new WaitForSeconds(1.5f);
        _playerGrid.ShowUnitAffectedArea(position);
        _showUnitAffectedAreaDelayedCoroutine = null;
    }

    private void OnDestroy()
    {
        HexPointer.Instance.OnPositionChanged -= HandleUnits_OnPositionChanged;
    }
}
