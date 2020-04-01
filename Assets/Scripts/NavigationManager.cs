using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class NavigationManager : MonoBehaviour
{
    public Transform[] destinations; // list of destination positions
    private NavMeshPath path; // current calculated path
    public LineRenderer line; // linerenderer to display path
    public Transform target; // current chosen destination

    public Camera arCamera;
    public Transform personIndicator;
    public PositionConstraint personPositionConstraint;

    public Camera topDownCamera;
    public NavMeshSurface navMeshSurface;

    private Vector3 currentDestination;

    float pressingTimer = 0f;
    public float maxPressingTime = 0.5f;

    bool isDestinationSet = false;

    //create initial path, get linerenderer.
    void Start()
    {
        path = new NavMeshPath();
        line.enabled = false;
    }

    public void StartNavigation()
    {
        personIndicator.position = new Vector3(arCamera.transform.position.x, personIndicator.position.y, arCamera.transform.position.z);
        personPositionConstraint.constraintActive = true;
        navMeshSurface.BuildNavMesh();
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (pressingTimer < maxPressingTime)
            {
                pressingTimer += Time.deltaTime;
            }
            else
            {
                SetDestination(Input.mousePosition);
                pressingTimer = 0f;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressingTimer = 0f;
        }

        if (isDestinationSet)
        {
            CalculatePath();
            //CheckIfDestinationReached();
        }

    }

    private void CheckIfDestinationReached()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if point pressed on the screen is available as destination in the map
    /// </summary>
    /// <param name="mousePosition"></param>
    public void SetDestination(Vector2 mousePosition)
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
                currentDestination = navMeshHit.position;
                line.enabled = true;
                isDestinationSet = true;
            }
        }
    }

    /// <summary>
    /// Draws the path both in AR and in the map
    /// </summary>
    private void CalculatePath()
    {
        NavMesh.CalculatePath(personIndicator.position, currentDestination, NavMesh.AllAreas, path);
        //lost path due to standing above obstacle (drift)
        if (path.corners.Length == 0)
        {
            Debug.Log("Try moving away for obstacles (optionally recalibrate)");
        }
        line.positionCount = path.corners.Length;
        line.SetPositions(path.corners);
        //line.enabled = true;
    }
}
