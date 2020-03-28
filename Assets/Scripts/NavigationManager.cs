using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    public Transform[] destinations; // list of destination positions
    public GameObject person; // person indicator
    private NavMeshPath path; // current calculated path
    public LineRenderer line; // linerenderer to display path
    public Transform target; // current chosen destination
    private bool destinationSet; // bool to say if a destination is set


    //create initial path, get linerenderer.
    void Start()
    {
        path = new NavMeshPath();
        destinationSet = false;
    }

    void Update()
    {
        //if a target is set, calculate and update path
        if (target != null)
        {
            NavMesh.CalculatePath(person.transform.position, target.position,
                          NavMesh.AllAreas, path);
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
}
