using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CalibrationManager : MonoBehaviour
{
    [Header("Image related references")]
    public ARTrackedImageManager imageManager;
    public XRReferenceImageLibrary trackedImagesLibrary;
    static Guid[] trackedImagesIdentifiers;
    public Transform[] virtualImagesTrs;

    [Header("Other references")]
    public GameObject levelObject;
    public NavigationManager navigationManager;
    public Button calibrateButton;

    [Header("Calibration parameters")]
    bool isCalibrationReady = false;
    bool isCalibrating = false;
    public float calibrationDuration = 2f;
    float calibrationTimer = 0.0f;

    void OnEnable()
    {
        calibrateButton.onClick.AddListener(SetCalibrationReady);

        trackedImagesIdentifiers = new Guid[trackedImagesLibrary.count];
        for (int i = 0; i < trackedImagesLibrary.count; i++)
        {
            trackedImagesIdentifiers[i] = trackedImagesLibrary[i].guid;
        }

        imageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    private void Update()
    {
        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;
            if(calibrationTimer >= calibrationDuration)
            {
                calibrationTimer = 0f;
                FinishCalibration();
            }
        }
    }

    private void SetCalibrationReady()
    {
        isCalibrationReady = true;
        calibrateButton.interactable = false;
    }

    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        if (obj.updated.Count > 0 && isCalibrationReady)
        {
            isCalibrating = true;
            isCalibrationReady = false;
        }
        else if(obj.updated.Count > 0 && isCalibrating)
        {
            ARTrackedImage trackedImg = obj.updated[0];
            if (trackedImg.trackingState == TrackingState.Tracking)
            {
                int imgIndex = -1;

                for (int i = 0; i < trackedImagesIdentifiers.Length; i++)
                {
                    if (trackedImg.referenceImage.guid == trackedImagesIdentifiers[i])
                        imgIndex = i;
                }

                if (imgIndex == -1) return;

                Calibrate(trackedImg.transform, virtualImagesTrs[imgIndex]);
            }
        }
    }

    public void Calibrate(Transform realTransform, Transform virtualTransform)
    {
        CalibrateRotation(realTransform, virtualTransform, levelObject.transform);
        CalibratePosition(realTransform, virtualTransform, levelObject.transform);
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

    public void FinishCalibration()
    {
        isCalibrating = false;
        levelObject.SetActive(true);
        navigationManager.SetNavigationReady();
        calibrateButton.interactable = true;
    }
}
