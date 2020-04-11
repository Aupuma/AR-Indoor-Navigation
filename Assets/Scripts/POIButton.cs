using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIButton : MonoBehaviour
{
    public Image icon;
    public Image background;

    public Color selectedIconColor;
    public Color normalIconColor;

    public Color selectedBackgroundColor;
    public Color normalBackgroundColor;

    private int myIndex;

    private bool isSelected = false;

    public delegate void PoiButtonDelegate(int poiIndex);
    public PoiButtonDelegate OnPoiButtonPressed;

    public void SetData(Sprite sprite, int index)
    {
        icon.sprite = sprite;
        myIndex = index;
    }

    public void Select()
    {
        if (isSelected) return;

        isSelected = true;
        OnPoiButtonPressed?.Invoke(myIndex);
        icon.color = selectedIconColor;
        background.color = selectedBackgroundColor;
    }

    public void Deselect()
    {
        isSelected = false;
        icon.color = normalIconColor;
        background.color = normalBackgroundColor;
    }
}
