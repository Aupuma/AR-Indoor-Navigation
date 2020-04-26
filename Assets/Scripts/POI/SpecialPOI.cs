using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;

public class SpecialPOI : MonoBehaviour
{
    [SerializeField] AimConstraint _arPinAimConstraint;
    [SerializeField] RotationConstraint _minimapPinRotationConstraint;
    [SerializeField] ScaleWithZoomConstraint _largeMapPinZoomConstraint;

    [SerializeField] Camera _arCamera;
    [SerializeField] Camera _minimapCamera;
    [SerializeField] Camera _largeMapCamera;

    [SerializeField] Transform _largeMapObjectivePin;
    [SerializeField] ParticleSystem _targetArrivedPS;
    [SerializeField] Animator _arPinAnimator;

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

    public void PlaySetDestinationAnimation()
    {
        if(_largeMapObjectivePin != null)
            _largeMapObjectivePin.DOScale(0, 0.25f).From().SetEase(Ease.OutBack);
    }

    public void PlayReachedAnimations()
    {
        _arPinAnimator.SetTrigger("Arrived");
        _targetArrivedPS.Play();
        Invoke("Deactivate", 0.75f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
