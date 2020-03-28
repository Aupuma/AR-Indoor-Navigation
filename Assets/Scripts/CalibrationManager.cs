using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //void OnEnable()
    //{
    //    m_ImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    //}

    //void OnDisable()
    //{
    //    m_ImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    //}


    //void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    //{
    //    if (!isCalibrating) return;

    //    // updated, set prefab position and rotation
    //    foreach (ARTrackedImage image in obj.added)
    //    {
    //        // image is tracking or tracking with limited state, show visuals and update it's position and rotation
    //        if (image.trackingState == TrackingState.Tracking)
    //        {
    //            CalibratePosition(image.transform, childTransform);
    //            CalibrateRotation(image.transform, childTransform);
    //        }
    //    }

    //    foreach (ARTrackedImage image in obj.updated)
    //    {
    //        // image is tracking or tracking with limited state, show visuals and update it's position and rotation
    //        if (image.trackingState == TrackingState.Tracking)
    //        {
    //            CalibratePosition(image.transform, childTransform);
    //            CalibrateRotation(image.transform, childTransform);
    //        }
    //    }
    //}

    //public void ToggleCalibration(bool calibrate)
    //{
    //    isCalibrating = calibrate;
    //}

    public void Calibrate()
    {
        CalibrateRotation(targetTransform, childTransform);
        CalibratePosition(targetTransform, childTransform);
        scene.SetActive(true);
        //calibrationCanvas.SetActive(false);
    }

    public void CalibratePosition(Transform targetTransform, Transform childTransform)
    {
        Vector3 differenceVector = targetTransform.position - childTransform.position;
        childTransform.parent.position = new Vector3(childTransform.parent.position.x + differenceVector.x, childTransform.parent.position.y + differenceVector.y, childTransform.parent.position.z + differenceVector.z);
    }

    public void CalibrateRotation(Transform targetTransform, Transform childTransform)
    {
        //APPLYING CORRECT ROTATION METHODS------------------------------------------------------
        // Populate the net rotation that you want the child to have.
        Quaternion desiredRotation = Quaternion.LookRotation(targetTransform.forward, Vector3.up);

        // Create a rotation that undoes the child's rotation, then applies the desired rotation.
        Quaternion rotationCorrection = desiredRotation * Quaternion.Inverse(childTransform.localRotation);

        // The parent will apply this correction to its child transforms.
        Vector3 convertedCorrection = Quaternion.ToEulerAngles(rotationCorrection);

        //The object will only change its yaw
        childTransform.parent.eulerAngles = new Vector3(0, convertedCorrection.y * Mathf.Rad2Deg, 0);
        //----------------------------------------------------------------------------------------
    }
}
