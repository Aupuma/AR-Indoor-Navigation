using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("Parameters")]
    // change variables by preference
    public float orthoZoomSpeed = 0.01f;
    public float zoomOutMin = 5;
    public float zoomOutMax = 15;

    public float minFingerMovementToMoveMap = 20f;
    private Vector3 touchStartWorld; // start of finger touch
    private Vector3 touchStartScreen;

    public float pressingTimeToChooseDestination = 0.5f;
    float pressingTimer = 0f;

    bool hasMovedCamera = false;
    public Color recenterInactiveButtonColor;
    public Color recenterActiveButtonColor;
    public float recenterCameraDuration = 1f;

    [Header("References")]
    public Transform topDownCameraOrigin;
    public Camera topDownCamera;
    public NavigationManager navigationManager;
    public Image centerCameraIcon;

    void Update()
    {
        // mouse of fingertouch moves camera
        if (Input.GetMouseButtonDown(0))
        {
            touchStartScreen = Input.mousePosition;
            touchStartWorld = topDownCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            TouchscreenZoom();
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 fingerMovement = touchStartScreen - Input.mousePosition;

            //If the finger does not practically move, a destination is chosen
            if (fingerMovement.magnitude < minFingerMovementToMoveMap)
            {
                if (pressingTimer < pressingTimeToChooseDestination)
                {
                    pressingTimer += Time.deltaTime;
                }
                else
                {
                    navigationManager.SetDestination(Input.mousePosition);
                    pressingTimer = 0f;
                }
            }
            else //Otherwise the map is moved proportionally to the distance the finger moved
            {
                Move(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressingTimer = 0f;
        }

        //zoom with scroll wheel
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
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

        Zoom(deltaMagnitudeDiff * orthoZoomSpeed);
    }

    // zoom with camera
    private void Zoom(float incr)
    {
        topDownCamera.orthographicSize = Mathf.Clamp(topDownCamera.orthographicSize - incr, zoomOutMin, zoomOutMax);
    }

    private void Move(Vector2 currentPos)
    {
        if (!hasMovedCamera)
        {
            topDownCamera.transform.parent = null;
            hasMovedCamera = true;
            centerCameraIcon.color = recenterActiveButtonColor;
        }

        DOTween.KillAll();

        Vector3 direction = touchStartWorld - topDownCamera.ScreenToWorldPoint(currentPos);
        topDownCamera.transform.position += direction;
    }

    public void MoveCameraToCenter()
    {
        if (hasMovedCamera)
        {
            topDownCamera.transform.DOMove(topDownCameraOrigin.position, recenterCameraDuration).SetEase(Ease.OutCirc).
                OnComplete(RecenterCamera);
        }
    }

    public void RecenterCamera()
    {
        hasMovedCamera = false;
        DOTween.KillAll();

        topDownCamera.transform.position = topDownCameraOrigin.position;
        topDownCamera.transform.SetParent(topDownCameraOrigin);
        centerCameraIcon.color = recenterInactiveButtonColor;
    }
}
