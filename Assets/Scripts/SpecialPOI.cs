using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SpecialPOI : MonoBehaviour
{
    [SerializeField] AimConstraint _arPinAimConstraint;
    [SerializeField] RotationConstraint _minimapPinRotationConstraint;
    [SerializeField] ScaleWithZoomConstraint _largeMapPinZoomConstraint;

    [SerializeField] Camera _arCamera;
    [SerializeField] Camera _minimapCamera;
    [SerializeField] Camera _largeMapCamera;

    private void Start()
    {
        SetConstraints(_arCamera, _minimapCamera, _largeMapCamera);
    }

    public void SetConstraints(Camera arCamera, Camera minimapCamera, Camera largeMapCamera)
    {
        ConstraintSource arSource = new ConstraintSource();
        arSource.sourceTransform = arCamera.transform;
        arSource.weight = 1;
        _arPinAimConstraint.AddSource(arSource);
        _arPinAimConstraint.constraintActive = true;

        ConstraintSource minimapSource = new ConstraintSource();
        minimapSource.sourceTransform = minimapCamera.transform;
        minimapSource.weight = 1;
        _minimapPinRotationConstraint.AddSource(minimapSource);
        _minimapPinRotationConstraint.constraintActive = true;

        _largeMapPinZoomConstraint.ConstraintCamera = largeMapCamera;
    }
}
