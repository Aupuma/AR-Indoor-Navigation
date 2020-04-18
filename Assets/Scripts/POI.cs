using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class POI : MonoBehaviour
{
    [SerializeField] Sprite _pinSprite;
    [SerializeField] Color _pinColor;

    [Header("AR components")]
    [SerializeField] AimConstraint _arPinAimConstraint;
    [SerializeField] MeshRenderer _arSymbol;

    [Header("Minimap components")]
    [SerializeField] RotationConstraint _minimapPinRotationConstraint;
    [SerializeField] MeshRenderer _minimapPin;
    [SerializeField] MeshRenderer _minimapSymbol;

    [Header("Large map components")]
    [SerializeField] Transform _largeMapPinParent;
    [SerializeField] float _scaleMultiplier;
    [SerializeField] MeshRenderer _largeMapPin;
    [SerializeField] MeshRenderer _largeMapSymbol;

    Camera _largeMapCamera;

    // Start is called before the first frame update
    void Start()
    {
        _arSymbol.material.mainTexture = _pinSprite.texture;
        _minimapSymbol.material.mainTexture = _pinSprite.texture;
        _largeMapSymbol.material.mainTexture = _pinSprite.texture;

        _minimapPin.material.color = _pinColor;
        _largeMapPin.material.color = _pinColor;
    }

    private void Update()
    {
        if(_largeMapCamera != null)
            _largeMapPinParent.localScale = Vector3.one * _scaleMultiplier * _largeMapCamera.orthographicSize;
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

        _largeMapCamera = largeMapCamera;
    }
}
