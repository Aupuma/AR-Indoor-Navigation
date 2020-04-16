using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class POI : MonoBehaviour
{
    [SerializeField] Sprite _sprite;
    public Sprite Sprite => _sprite;

    [Header("AR components")]
    [SerializeField] AimConstraint _arPinAimConstraint;
    [SerializeField] MeshRenderer _arSymbol;
    [SerializeField] MeshRenderer _arFloorIndicator;
    [SerializeField] Color _selectedFloorColor;

    [Header("Minimap components")]
    [SerializeField] RotationConstraint _minimapPinRotationConstraint;
    [SerializeField] MeshRenderer _minimapSymbol;
    [SerializeField] GameObject _minimapPin;
    [SerializeField] GameObject _objectivePin;

    Color _nonSelectedColor;

    // Start is called before the first frame update
    void Start()
    {
        _arSymbol.material.mainTexture = _sprite.texture;
        _minimapSymbol.material.mainTexture = _sprite.texture;
        _nonSelectedColor = _arFloorIndicator.material.color;
    }

    public void SetConstraints(Camera arCamera, Camera minimapCamera)
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
    }

    public void SetAsDestination()
    {
        _arFloorIndicator.material.color = _selectedFloorColor;
        _minimapPin.SetActive(false);
        _objectivePin.SetActive(true);
    }

    public void DeselectAsDestination()
    {
        _arFloorIndicator.material.color = _nonSelectedColor;
        _minimapPin.SetActive(true);
        _objectivePin.SetActive(false);
    }
}
