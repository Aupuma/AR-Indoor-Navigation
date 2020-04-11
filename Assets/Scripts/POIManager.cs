using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIManager : MonoBehaviour
{
    public POI[] pointsOfInterest;

    public POIButton poiButtonPrefab;
    private POIButton[] poiButtons;
    public Transform poiButtonsParent;

    public Camera topDownCamera;
    public Camera arCamera;

    public delegate void PoiSelectionDelegate(Vector3 position);
    public PoiSelectionDelegate OnPoiSelected;

    private int selectedPoiIndex = -1;

    private void Start()
    {
        poiButtons = new POIButton[pointsOfInterest.Length];

        for (int i = 0; i < pointsOfInterest.Length; i++)
        {
            POIButton buttonInstance = Instantiate(poiButtonPrefab, poiButtonsParent);
            buttonInstance.SetData(pointsOfInterest[i].sprite,i);
            buttonInstance.OnPoiButtonPressed += SelectPoi;
            poiButtons[i] = buttonInstance;
        }
    }

    private void SelectPoi(int index)
    {
        if(index != selectedPoiIndex && selectedPoiIndex != -1)
        {
            DeselectPoi(selectedPoiIndex);
        }

        selectedPoiIndex = index;
        pointsOfInterest[selectedPoiIndex].SetAsDestination();
        OnPoiSelected?.Invoke(pointsOfInterest[selectedPoiIndex].transform.position);
    }

    private void DeselectPoi(int index)
    {
        pointsOfInterest[index].DeselectAsDestination();
        poiButtons[index].Deselect();
    }

    public void DeselectPois()
    {
        for (int i = 0; i < pointsOfInterest.Length; i++)
        {
            DeselectPoi(i);
        }
        selectedPoiIndex = -1;
    }

    void Update()
    {
        UpdatePoiButtonsPosition();
    }

    private void UpdatePoiButtonsPosition()
    {
        for (int i = 0; i < pointsOfInterest.Length; i++)
        {
            Vector3 poiPosition = pointsOfInterest[i].transform.position;
            Vector3 poiButtonPosition = topDownCamera.WorldToViewportPoint(poiPosition);
            if (poiButtonPosition.x > 0 && poiButtonPosition.x < 1 &&
                poiButtonPosition.y > 0 && poiButtonPosition.y < 1) //Inside the view
            {
                poiButtons[i].gameObject.SetActive(true);
                poiButtons[i].transform.position = arCamera.ViewportToScreenPoint(poiButtonPosition);
            }
            else
            {
                poiButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
