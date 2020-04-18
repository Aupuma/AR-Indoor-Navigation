using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class NavigationManager : MonoBehaviour
{
    [Header("General references")]
    [SerializeField] NavMeshSurface _navMeshSurface;
    [SerializeField] Transform _userIndicator;
    [SerializeField] POIManager _poiManager;
    [SerializeField] GameObject _startPoint;
    [SerializeField] GameObject _finishPoint;

    [Header("Map")]
    [SerializeField] Camera _topDownCamera;
    [SerializeField] LineRenderer _topDownLine;

    [Header("Augmented reality")]
    [SerializeField] Camera _arCamera;
    [SerializeField] ARLinePooler _arLinePooler; 

    [Header("Parameters")]
    [SerializeField] float _distanceToEndNavigation;
    [SerializeField] float _arLineHeight;

    NavMeshPath _path;
    Vector3 _currentDestination;
    bool _isDestinationSet = false;

    //create initial path, get linerenderer.
    void Start()
    {
        _path = new NavMeshPath();
        _poiManager.OnPoiSelected += CalculatePathTo;
        _topDownLine.gameObject.SetActive(false);
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
            if (Vector3.Distance(_userIndicator.position, _currentDestination) < _distanceToEndNavigation)
            {
                EndNavigation();
            }
        }
    }


    /// <summary>
    /// Checks if point pressed on the screen is available as destination in the map
    /// </summary>
    /// <param name="mousePosition"></param>
    public void SetManualDestination(Vector2 mousePosition)
    {
        Vector2 rayVector = _arCamera.GetComponent<Camera>().ScreenToViewportPoint(mousePosition);
        Ray camRay = _topDownCamera.ViewportPointToRay(rayVector);
        RaycastHit hitInfo;
        int layer_mask = LayerMask.GetMask("floor");

        if(Physics.Raycast(camRay, out hitInfo, layer_mask))
        {
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(hitInfo.point, out navMeshHit, 0.5f, NavMesh.AllAreas))
            {
                _poiManager.DeselectPois();
                _finishPoint.SetActive(true);
                _finishPoint.transform.position = navMeshHit.position;
                CalculatePathTo(navMeshHit.position);
            }
        }
    }

    public void CalculatePathTo(Vector3 endPosition)
    {
        _currentDestination = endPosition;

        Vector3 originPoint = new Vector3(_userIndicator.position.x, _currentDestination.y, _userIndicator.position.z);
        NavMesh.CalculatePath(originPoint, _currentDestination, NavMesh.AllAreas, _path);

        _arLinePooler.SetLinePositions(_path.corners);
        _topDownLine.positionCount = _path.corners.Length;
        _topDownLine.SetPositions(_path.corners);

        _startPoint.transform.position = originPoint;

        StartNavigation();
    }

    private void StartNavigation()
    {
        _startPoint.SetActive(true);
        _topDownLine.gameObject.SetActive(true);
        _isDestinationSet = true;
    }

    private void EndNavigation()
    {
        _isDestinationSet = false;
        _poiManager.DeselectPois();
        _topDownLine.gameObject.SetActive(false);
        _arLinePooler.HideLines();
        _startPoint.SetActive(false);
        _finishPoint.SetActive(false);
    }
}
