using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject mapScreen;
    public GameObject calibrationScreen;

    public GameObject calibrationMarker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetMapScreenVisibility(bool visible)
    {
        mapScreen.SetActive(visible);
        mainScreen.SetActive(!visible);
    }

    public void SetCalibrationScreenVisibility(bool visible)
    {
        calibrationScreen.SetActive(visible);
        calibrationMarker.SetActive(visible);
        mainScreen.SetActive(!visible);
    }
}
