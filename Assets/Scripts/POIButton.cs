using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIButton : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] GameObject _normalImage;
    [SerializeField] GameObject _pinImage;

    int _myIndex;
    bool _isSelected = false;

    public delegate void PoiButtonDelegate(int poiIndex);
    public PoiButtonDelegate OnPoiButtonPressed;


    public void SetData(Sprite sprite, int index)
    {
        _icon.sprite = sprite;
        _myIndex = index;

        Deselect();
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        OnPoiButtonPressed?.Invoke(_myIndex);
        _pinImage.SetActive(true);
        _normalImage.SetActive(false);
    }

    public void Deselect()
    {
        _isSelected = false;
        _pinImage.SetActive(false);
        _normalImage.SetActive(true);
    }
}
