using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CalibrationManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Image manager on the AR Session Origin")]
    ARTrackedImageManager m_ImageManager;

    /// <summary>
    /// Get the <c>ARTrackedImageManager</c>
    /// </summary>
    public ARTrackedImageManager ImageManager
    {
        get => m_ImageManager;
        set => m_ImageManager = value;
    }

    public Transform targetTransform;
    public Transform childTransform;
    public GameObject scene;
    public GameObject calibrationCanvas;
    bool isCalibrating = false;
    public NavMeshSurface surface;
    public NavigationManager navigationManager;


    void OnEnable()
    {
        m_ImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_ImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }


    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        if (!isCalibrating) return;
        else
        {
            foreach (ARTrackedImage image in obj.updated)
            {
                // image is tracking or tracking with limited state, show visuals and update it's position and rotation
                if (image.trackingState == TrackingState.Tracking)
                {
                    isCalibrating = false;
                    CalibratePosition(image.transform, childTransform, scene.transform);
                    CalibrateRotation(image.transform, childTransform, scene.transform);
                    Calibrate();
                }
            }
        }


    }

    public void TryCalibration()
    {
        isCalibrating = true;
    }

    public void Calibrate()
    {
        //CalibrateRotation(targetTransform, childTransform,scene.transform);
        //CalibratePosition(targetTransform, childTransform,scene.transform);
        scene.SetActive(true);
        surface.BuildNavMesh();
        navigationManager.StartNavigation();
    }

    public void CalibratePosition(Transform realMarker, Transform virtualMarker, Transform level)
    {
        Vector3 translationVector = realMarker.position - virtualMarker.position;
        level.Translate(translationVector, Space.World);
    }

    public void CalibrateRotation(Transform realMarker, Transform virtualMarker, Transform level)
    {
        // Populate the net rotation that you want the child to have.
        Quaternion desiredRotation = Quaternion.LookRotation(realMarker.up * -1, Vector3.up);

        // Create a rotation that undoes the child's rotation, then applies the desired rotation.
        Quaternion rotationCorrection = desiredRotation * Quaternion.Inverse(virtualMarker.localRotation);

        // The parent will apply this correction to its child transforms.
        Vector3 convertedCorrection = rotationCorrection.eulerAngles;

        //The object will only change its yaw
        level.eulerAngles = new Vector3(0, convertedCorrection.y, 0);
    }
}
