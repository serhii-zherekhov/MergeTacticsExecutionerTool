using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _enemyCamera;

    public static Pointer Instance { get; private set; }

    private Vector3 _screenPosition = Vector3.zero;
    private Vector3 _worldPosition = Vector3.zero;
    private Plane _plane = new Plane(Vector3.down, 0);
    private Ray _ray = new Ray();

    private ScreenHalf _screenHalf;

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

    private void Update()
    {
        _screenPosition = Input.mousePosition;

        if((_screenPosition.x / Screen.width) < 0.5f)
        {
            _screenHalf = ScreenHalf.Player;
        }
        else
        {
            _screenHalf = ScreenHalf.Enemy;
        }

        if (_screenHalf == ScreenHalf.Player)
        {
            _ray = _playerCamera.ScreenPointToRay(_screenPosition);
        }

        if(_screenHalf == ScreenHalf.Enemy)
        {
            _ray = _enemyCamera.ScreenPointToRay(_screenPosition);
        }

        if (_plane.Raycast(_ray, out float distance))
        {
            _worldPosition = _ray.GetPoint(distance);
        }
    }

    public Vector3 GetWorldPosition()
    {
        return _worldPosition;
    }

    private bool IsMouseOverLayer(int layer)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        foreach (RaycastResult raycastResult in raycastResultList)
        {
            if (raycastResult.gameObject.layer == layer)
            {
                return true;
            }
        }

        return false;
    }

    #region Properties
    public bool IsMouseOverUI
    {
        get
        {
            return IsMouseOverLayer(Layer.UI);
        }
    }

    public Vector3 RayVector
    {
        get
        {
            return _ray.direction;
        }
    }

    public ScreenHalf ScreenHalf
    {
        get
        {
            return _screenHalf;
        }
    }
    #endregion
}
