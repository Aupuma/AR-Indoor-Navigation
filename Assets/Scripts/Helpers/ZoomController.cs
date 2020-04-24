using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{

    [SerializeField] float _orthoZoomSpeed = 0.01f;
    [SerializeField] float _zoomOutMin = 5;
    [SerializeField] float _zoomOutMax = 15;
    [SerializeField] Camera _topDownCamera;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //zoom with scroll wheel
        Zoom(Input.GetAxis("Mouse ScrollWheel"));

#elif UNITY_ANDROID
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            TouchscreenZoom();
        }
#endif
    }

    private void TouchscreenZoom()
    {
        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        Zoom(deltaMagnitudeDiff * _orthoZoomSpeed);
    }


    // zoom with camera
    private void Zoom(float incr)
    {
        _topDownCamera.orthographicSize = Mathf.Clamp(_topDownCamera.orthographicSize - incr, _zoomOutMin, _zoomOutMax);
    }

}
