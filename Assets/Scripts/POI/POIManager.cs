using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIManager : MonoBehaviour
{
    [SerializeField] POI[] _pointsOfInterest;

    [Header("Cameras")]
    [SerializeField] Camera _largeMapCamera;
    [SerializeField] Camera _arCamera;
    [SerializeField] Camera _minimapCamera;

    POI _selectedPoi;

    public delegate void PoiSelectionDelegate(Vector3 position);
    public PoiSelectionDelegate OnPoiSelected;

    private void Start()
    {
        for (int i = 0; i < _pointsOfInterest.Length; i++)
        {
            _pointsOfInterest[i].SetConstraints(_arCamera, _minimapCamera,_largeMapCamera);
        }
    }

    public void PlayPOIReachedAnimation()
    {
        _selectedPoi.PlayReachedAnimations();
    }

    public void DeselectPoi()
    {
        if (_selectedPoi != null)
        {
            _selectedPoi.Deselect();
            _selectedPoi = null;
        }
    }

    public void ActivateMinimapCamera()
    {
        _largeMapCamera.enabled = false;
        _minimapCamera.enabled = true;
    }

    public void ActivateLargeMapCamera()
    {
        _largeMapCamera.enabled = true;
        _minimapCamera.enabled = false;
    }

    void Update()
    {
        if (_largeMapCamera.enabled && Input.GetMouseButtonDown(0))
        {
            RaycastForPoi(Input.mousePosition);
        }
    }

    private void RaycastForPoi(Vector2 mousePosition)
    {
        Vector2 rayVector = _arCamera.ScreenToViewportPoint(mousePosition);
        Ray camRay = _largeMapCamera.ViewportPointToRay(rayVector);
        RaycastHit hitInfo;
        int layer_mask = LayerMask.GetMask("Map");

        if (Physics.Raycast(camRay, out hitInfo, layer_mask))
        {
            if(hitInfo.collider.GetComponentInParent<POI>() != null)
            {
                DeselectPoi();
                _selectedPoi = hitInfo.collider.GetComponentInParent<POI>();
                _selectedPoi.Select();
                OnPoiSelected?.Invoke(_selectedPoi.transform.position);
            }
        }
    }
}
