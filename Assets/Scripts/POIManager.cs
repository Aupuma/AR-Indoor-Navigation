using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIManager : MonoBehaviour
{
    [SerializeField] POI[] _pointsOfInterest;

    [SerializeField] POIButton _poiButtonPrefab;
    [SerializeField] Transform _poiButtonsParent;

    [SerializeField] RawImage _largeMap;
    [SerializeField] Camera _largeMapCamera;

    [SerializeField] Camera _arCamera;

    [SerializeField] Camera _minimapCamera;

    POIButton[] _poiButtons;
    int _selectedPoiIndex = -1;

    public delegate void PoiSelectionDelegate(Vector3 position);
    public PoiSelectionDelegate OnPoiSelected;

    private void Start()
    {
        _poiButtons = new POIButton[_pointsOfInterest.Length];

        for (int i = 0; i < _pointsOfInterest.Length; i++)
        {
            _pointsOfInterest[i].SetConstraints(_arCamera, _minimapCamera);
            POIButton buttonInstance = Instantiate(_poiButtonPrefab, _poiButtonsParent);
            buttonInstance.SetData(_pointsOfInterest[i].Sprite,i);
            buttonInstance.OnPoiButtonPressed += SelectPoi;
            _poiButtons[i] = buttonInstance;
        }
    }

    private void SelectPoi(int index)
    {
        if(index != _selectedPoiIndex && _selectedPoiIndex != -1)
        {
            DeselectPoi(_selectedPoiIndex);
        }

        _selectedPoiIndex = index;
        _pointsOfInterest[_selectedPoiIndex].SetAsDestination();
        OnPoiSelected?.Invoke(_pointsOfInterest[_selectedPoiIndex].transform.position);
    }

    private void DeselectPoi(int index)
    {
        _pointsOfInterest[index].DeselectAsDestination();
        _poiButtons[index].Deselect();
    }

    public void DeselectPois()
    {
        for (int i = 0; i < _pointsOfInterest.Length; i++)
        {
            DeselectPoi(i);
        }
        _selectedPoiIndex = -1;
    }

    public void SwitchToMinimapMode()
    {
        _largeMapCamera.enabled = false;
        _minimapCamera.enabled = true;
    }

    public void SwitchToLargeMapMode()
    {
        _largeMapCamera.enabled = true;
        _minimapCamera.enabled = false;
    }

    void Update()
    {
        if(_largeMapCamera.enabled)
            UpdatePoiButtonsPosition();
    }


    /// <summary>
    /// 
    /// </summary>
    private void UpdatePoiButtonsPosition()
    {
        for (int i = 0; i < _pointsOfInterest.Length; i++)
        {
            Vector3 poiPosition = _pointsOfInterest[i].transform.position;
            Vector3 poiViewportPosition = _largeMapCamera.WorldToViewportPoint(poiPosition);

            if (poiViewportPosition.x > 0 && poiViewportPosition.x < 1 &&
                poiViewportPosition.y > 0 && poiViewportPosition.y < 1) //Inside the viewport of the camera
            {
                _poiButtons[i].gameObject.SetActive(true);
                _poiButtons[i].transform.position = _arCamera.ViewportToScreenPoint(poiViewportPosition);
            }
            else
            {
                _poiButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
