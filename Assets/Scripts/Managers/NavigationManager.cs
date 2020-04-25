using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    #region SINGLETON
    public static NavigationManager instance;

    private void Awake()
    {
        instance = this;
    } 
    #endregion

    [Header("General references")]
    [SerializeField] NavMeshSurface _navMeshSurface;
    [SerializeField] Transform _userIndicator;
    [SerializeField] POIManager _poiManager;
    [SerializeField] GameObject _startPoint;
    [SerializeField] GameObject _finishPoint;

    [Header("Map")]
    [SerializeField] Camera _topDownCamera;
    [SerializeField] LinePool _topDownLinePool;

    [Header("Augmented reality")]
    [SerializeField] Camera _arCamera;
    [SerializeField] LinePool _arLinePool;
    [SerializeField] Target _targetPointObject;

    [Header("Parameters")]
    [SerializeField] float _distanceToEndNavigation;
    [SerializeField] float _arLineHeight;
    [SerializeField] float _distanceToIncreaseTargetPoint;

    NavMeshPath _path;
    Vector3 _currentDestination;
    bool _isDestinationSet = false;
    int _targetPointIndex = -1;
    Vector3[] _currentPathPoints;

    //create initial path, get linerenderer.
    void Start()
    {
        _path = new NavMeshPath();
        _poiManager.OnPoiSelected += SetPoiAsDestination;
        _arLinePool.LineHeight = _arLineHeight;
    }

    public void SetNavigationReady()
    {
        _userIndicator.position = new Vector3(_arCamera.transform.position.x, _userIndicator.position.y, _arCamera.transform.position.z);
        _navMeshSurface.BuildNavMesh();
    }

    void Update()
    {
        if (_isDestinationSet)
        {
            UpdateCurrentPoint();

            if (Vector3.Distance(_userIndicator.position, _currentDestination) < _distanceToEndNavigation)
            {
                EndNavigation();
            }

            UpdateOffScreenPointerVisibility();
        }
    }

    private void UpdateOffScreenPointerVisibility()
    {
        Vector3 targetPositionScreenPoint = _arCamera.WorldToScreenPoint(_targetPointObject.transform.position);
        bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width ||
            targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;

        _targetPointObject.enabled = isOffScreen;
    }

    private void UpdateCurrentPoint()
    {
        if (_targetPointIndex < _currentPathPoints.Length - 2)
        {
            if (Vector3.Distance(_userIndicator.position, _currentPathPoints[_targetPointIndex]) < _distanceToIncreaseTargetPoint)
            {
                _targetPointIndex++;
                _targetPointObject.transform.position = _currentPathPoints[_targetPointIndex];
            }
        }
    }

    /// <summary>
    /// Checks if point pressed on the screen is available as destination in the map
    /// </summary>
    /// <param name="mousePosition"></param>
    public void SetManualDestination(Vector2 mousePosition)
    {
        Vector2 rayVector = _arCamera.ScreenToViewportPoint(mousePosition);
        Ray camRay = _topDownCamera.ViewportPointToRay(rayVector);
        RaycastHit hitInfo;
        int layer_mask = LayerMask.GetMask("floor");

        if(Physics.Raycast(camRay, out hitInfo, layer_mask))
        {
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(hitInfo.point, out navMeshHit, 0.5f, NavMesh.AllAreas))
            {
                _poiManager.DeselectPoi();
                _finishPoint.SetActive(true);
                _finishPoint.transform.position = navMeshHit.position;
                SetDestination(navMeshHit.position);
            }
        }
    }

    private void SetPoiAsDestination(Vector3 poiPosition)
    {
        _finishPoint.SetActive(false);
        SetDestination(poiPosition);
    }

    /// <summary>
    /// Given a destination in the map, calculates the path to it and 
    /// displays it in maps and AR mode
    /// </summary>
    /// <param name="endPosition"></param>
    public void SetDestination(Vector3 endPosition)
    {
        _isDestinationSet = true;
        _currentDestination = endPosition;

        Vector3 originPoint = new Vector3(_userIndicator.position.x, _currentDestination.y, _userIndicator.position.z);
        NavMesh.CalculatePath(originPoint, _currentDestination, NavMesh.AllAreas, _path);
        _arLinePool.SetLinePositions(_path.corners);
        _topDownLinePool.SetLinePositions(_path.corners);

        _currentPathPoints = _path.corners;
        _targetPointIndex = 1;
        _targetPointObject.enabled = true;
        _targetPointObject.transform.position = _currentPathPoints[_targetPointIndex];

        _startPoint.SetActive(true);
        _startPoint.transform.position = originPoint;
    }

    public void EndNavigation()
    {
        if (!_isDestinationSet) return;

        _isDestinationSet = false;

        _poiManager.DeselectPoi();
        _topDownLinePool.HideLines();
        _arLinePool.HideLines();

        _currentPathPoints = null;
        _targetPointIndex = -1;
        _targetPointObject.enabled = false;

        _startPoint.SetActive(false);
        _finishPoint.SetActive(false);
    }
}
