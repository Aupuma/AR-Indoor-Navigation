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
    //EDITABLE FIELDS
    [Header("Image related references")]
    [SerializeField] ARTrackedImageManager _imageManager;
    [SerializeField] XRReferenceImageLibrary _trackedImagesLibrary;
    [SerializeField] Transform[] _virtualImagesTrs;

    [Header("Other references")]
    [SerializeField] GameObject _levelObject;
    [SerializeField] Animator _calibrationIndicator;

    [Header("Calibration parameters")]
    [SerializeField] float _calibrationDuration = 2f;

    //VARIABLES
    static Guid[] _trackedImagesIdentifiers;
    ARTrackedImage _currentTrackedImage = null;
    int _currentImageIndex = -1;

    bool _isCalibrating = false;
    float _calibrationTimer = 0.0f;

    private void Start()
    {
        _trackedImagesIdentifiers = new Guid[_trackedImagesLibrary.count];
        for (int i = 0; i < _trackedImagesLibrary.count; i++)
        {
            _trackedImagesIdentifiers[i] = _trackedImagesLibrary[i].guid;
        }
    }

    void OnEnable()
    {
        _imageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
        CanvasManager.instance.ShowCalibrationState("Looking for a marker...");
        CanvasManager.instance.SetFindMarkerImgVisibility(true);
        _levelObject.SetActive(false);
    }

    void OnDisable()
    {
        _imageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    private void Update()
    {
        if (_isCalibrating)
        {
            _calibrationTimer += Time.deltaTime;
            if(_calibrationTimer >= _calibrationDuration)
            {
                _calibrationTimer = 0f;
                FinishCalibration();
            }
        }
    }

    /// <summary>
    /// Detects if there is a marker being added or updated
    /// and calibrates according to the marker
    /// </summary>
    /// <param name="obj"></param>
    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        if(obj.updated.Count > 0)
        {
            if (!_isCalibrating)
            {
                //Checks if there is an image tracked and visible on screen
                foreach (ARTrackedImage image in obj.updated)
                {
                    if (image.trackingState == TrackingState.Tracking)
                        _currentTrackedImage = image;
                }
                if (_currentTrackedImage == null) return;

                //Checks if the tracked image on screen is on the image database
                for (int i = 0; i < _trackedImagesIdentifiers.Length; i++)
                {
                    if (_currentTrackedImage.referenceImage.guid == _trackedImagesIdentifiers[i])
                        _currentImageIndex = i;
                }
                if (_currentImageIndex == -1) return;

                StartCalibration();
            }
            else
            {
                if (_currentTrackedImage.trackingState == TrackingState.Tracking)
                {
                    Calibrate(_currentTrackedImage.transform, _virtualImagesTrs[_currentImageIndex]);
                }
            }
        }
    }

    private void StartCalibration()
    {
        _calibrationIndicator.transform.SetParent(_currentTrackedImage.transform, false);
        _calibrationIndicator.SetTrigger("StartCalibration");
        CanvasManager.instance.ShowCalibrationState("Calibrating...");
        CanvasManager.instance.SetFindMarkerImgVisibility(false);
        _isCalibrating = true;
    }

    public void Calibrate(Transform realTransform, Transform virtualTransform)
    {
        CalibrateRotation(realTransform, virtualTransform, _levelObject.transform);
        CalibratePosition(realTransform, virtualTransform, _levelObject.transform);
    }

    /// <summary>
    /// Applies to the level transform a traslation so the position of the virtual marker
    /// matches the position of the real marker
    /// </summary>
    /// <param name="realMarker"></param>
    /// <param name="virtualMarker"></param>
    /// <param name="level"></param>
    public void CalibratePosition(Transform realMarker, Transform virtualMarker, Transform level)
    {
        Vector3 translationVector = realMarker.position - virtualMarker.position;
        level.Translate(translationVector, Space.World);
    }

    /// <summary>
    /// Applies to the level transform a rotation so the rotation of the virtual marker
    /// matches the rotation of the real marker
    /// </summary>
    /// <param name="realMarker"></param>
    /// <param name="virtualMarker"></param>
    /// <param name="level"></param>
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
        _isCalibrating = false;
        _currentTrackedImage = null;
        _levelObject.SetActive(true);
        _calibrationIndicator.SetTrigger("FinishCalibration");
        CanvasManager.instance.ShowCalibrationState("Calibration complete", true);
        CanvasManager.instance.SwitchState(AppState.NAVIGATION);
    }
}
