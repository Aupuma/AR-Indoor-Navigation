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
    [SerializeField] GameObject _minimapObjective;

    [Header("Large map components")]
    [SerializeField] ScaleWithZoomConstraint _largeMapPinZoomConstraint;
    [SerializeField] MeshRenderer _largeMapPin;
    [SerializeField] MeshRenderer _largeMapSymbol;
    [SerializeField] GameObject _largeMapObjective;

    bool _isSelected = false;
    public bool IsSelected => _isSelected;

    // Start is called before the first frame update
    void Start()
    {
        _arSymbol.material.mainTexture = _pinSprite.texture;
        _minimapSymbol.material.mainTexture = _pinSprite.texture;
        _largeMapSymbol.material.mainTexture = _pinSprite.texture;

        _minimapPin.material.color = _pinColor;
        _largeMapPin.material.color = _pinColor;
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

    public void Select()
    {
        if (_isSelected) return;

        _minimapObjective.SetActive(true);
        _minimapPin.gameObject.SetActive(false);

        _largeMapObjective.SetActive(true);
        _largeMapPin.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        _minimapObjective.SetActive(false);
        _minimapPin.gameObject.SetActive(true);

        _largeMapObjective.SetActive(false);
        _largeMapPin.gameObject.SetActive(true);
    }
}
