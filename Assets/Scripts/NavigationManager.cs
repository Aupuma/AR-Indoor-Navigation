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
    NavMeshPath path; // current calculated path

    public Camera topDownCamera;
    public LineRenderer topDownLine;

    public Camera arCamera;
    public ARLinePooler arLinePooler; 

    public Transform personIndicator;
    public PositionConstraint personPositionConstraint;

    public POIManager poiManager;

    [Header("Parameters")]
    public float distanceToEndNavigation;

    Vector3 currentDestination;
    bool isDestinationSet = false;

    //create initial path, get linerenderer.
    void Start()
    {
        path = new NavMeshPath();
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
        if (isDestinationSet)
        {
            if (Vector3.Distance(personIndicator.position, currentDestination) < distanceToEndNavigation)
            {
                isDestinationSet = false;
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
        currentDestination = worldPosition;
        isDestinationSet = true;

        topDownLine.gameObject.SetActive(true);

        Vector3 arOriginPoint = new Vector3(personIndicator.position.x, currentDestination.y, personIndicator.position.y);
        NavMesh.CalculatePath(personIndicator.position, currentDestination, NavMesh.AllAreas, path);
        arLinePooler.SetLinePositions(path.corners);
    }


    private void UpdateTopDownPath()
    {
        NavMesh.CalculatePath(personIndicator.position, currentDestination, NavMesh.AllAreas, path);
        topDownLine.positionCount = path.corners.Length;
        topDownLine.SetPositions(path.corners);
    }
}
