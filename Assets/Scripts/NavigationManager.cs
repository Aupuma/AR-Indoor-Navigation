using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class NavigationManager : MonoBehaviour
{
    [Header("References")]
    public NavMeshSurface navMeshSurface;
    NavMeshPath _path; // current calculated path

    public Camera topDownCamera;
    public LineRenderer topDownLine;

    public Camera arCamera;
    public ARLinePooler arLinePooler; 

    public Transform personIndicator;
    public PositionConstraint personPositionConstraint;

    public POIManager poiManager;

    [Header("Parameters")]
    [SerializeField] float _distanceToEndNavigation;

    Vector3 _currentDestination;
    bool _isDestinationSet = false;

    //create initial path, get linerenderer.
    void Start()
    {
        _path = new NavMeshPath();
        poiManager.OnPoiSelected += SetDestination;
        topDownLine.gameObject.SetActive(false);
    }

    public void StartNavigation()
    {
        personIndicator.position = new Vector3(arCamera.transform.position.x, personIndicator.position.y, arCamera.transform.position.z);
        personPositionConstraint.constraintActive = true;
        navMeshSurface.BuildNavMesh();
    }

    void Update()
    {
        if (_isDestinationSet)
        {
            if (Vector3.Distance(personIndicator.position, _currentDestination) < _distanceToEndNavigation)
            {
                _isDestinationSet = false;
                poiManager.DeselectPois();
                topDownLine.gameObject.SetActive(false);
                arLinePooler.HideLines();
            }
            else
            {
                UpdateTopDownPath();
            }
        }
    }


    /// <summary>
    /// Checks if point pressed on the screen is available as destination in the map
    /// </summary>
    /// <param name="mousePosition"></param>
    public void SetManualDestination(Vector2 mousePosition)
    {
        Vector2 rayVector = arCamera.GetComponent<Camera>().ScreenToViewportPoint(mousePosition);
        Ray camRay = topDownCamera.ViewportPointToRay(rayVector);
        RaycastHit hitInfo;
        int layer_mask = LayerMask.GetMask("floor");

        if(Physics.Raycast(camRay, out hitInfo, layer_mask))
        {
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(hitInfo.point, out navMeshHit, 0.5f, NavMesh.AllAreas))
            {
                poiManager.DeselectPois();
                SetDestination(navMeshHit.position);
            }
        }
    }


    /// <summary>
    /// Sets the destination point and starts navigation
    /// </summary>
    /// <param name="worldPosition"></param>
    public void SetDestination(Vector3 worldPosition)
    {
        _currentDestination = worldPosition;
        _isDestinationSet = true;

        topDownLine.gameObject.SetActive(true);

        Vector3 arOriginPoint = new Vector3(personIndicator.position.x, _currentDestination.y, personIndicator.position.y);
        NavMesh.CalculatePath(personIndicator.position, _currentDestination, NavMesh.AllAreas, _path);
        arLinePooler.SetLinePositions(_path.corners);
    }


    private void UpdateTopDownPath()
    {
        NavMesh.CalculatePath(personIndicator.position, _currentDestination, NavMesh.AllAreas, _path);
        topDownLine.positionCount = _path.corners.Length;
        topDownLine.SetPositions(_path.corners);
    }
}
