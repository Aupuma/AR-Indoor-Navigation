using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithZoomConstraint : MonoBehaviour
{
    [SerializeField] float _scaleMultiplier = 0.15f;

    [SerializeField] Camera _constraintCamera;
    public Camera ConstraintCamera { get => _constraintCamera; set => _constraintCamera = value; }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * _scaleMultiplier * _constraintCamera.orthographicSize;
    }
}
