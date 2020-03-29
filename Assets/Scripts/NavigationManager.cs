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
    private bool navigationReady = false; // bool to say if a destination is set


    public Transform arCamera;
    private Vector3 lastArCameraPosition;

    public Transform personIndicator;
    public PositionConstraint personPositionConstraint;

    public Camera topDownCamera;
    public NavMeshSurface navMeshSurface;

    bool isSettingDestination = true;

    private Vector3 currentDestination;

    //create initial path, get linerenderer.
    void Start()
    {
        path = new NavMeshPath();
        navigationReady = false;
        lastArCameraPosition = Vector3.zero;
    }

    public void StartNavigation()
    {
        Vector3 personStartPosition = new Vector3(arCamera.position.x, personIndicator.position.y, arCamera.position.z);
        personIndicator.position = personStartPosition;
        personPositionConstraint.constraintActive = true;
        isSettingDestination = true;
    }

    void Update()
    {
        if (isSettingDestination)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetDestination(Input.mousePosition);
            }
        }

        //if a target is set, calculate and update path
        if (navigationReady)
        {
            NavMesh.CalculatePath(personIndicator.position, currentDestination, NavMesh.AllAreas, path);
            //lost path due to standing above obstacle (drift)
            if (path.corners.Length == 0)
            {
                Debug.Log("Try moving away for obstacles (optionally recalibrate)");
            }
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
            line.enabled = true;
            
        }
    }

    public void SetDestination(Vector2 mousePosition)
    {
        Vector2 rayVector = new Vector2(mousePosition.x, mousePosition.y);
        Vector2 rayVectorTransformed = arCamera.GetComponent<Camera>().ScreenToViewportPoint(rayVector);

        Ray camRay = topDownCamera.ViewportPointToRay(rayVectorTransformed);
        RaycastHit hitInfo;
        int layer_mask = LayerMask.GetMask("floor");

        Debug.DrawRay(camRay.origin, camRay.direction, Color.red, 5f);
        if(Physics.Raycast(camRay, out hitInfo, layer_mask))
        {
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(hitInfo.point, out navMeshHit, 0.5f, NavMesh.AllAreas))
            {
                currentDestination = navMeshHit.position;
                navigationReady = true;
            }
        }
    }
}
