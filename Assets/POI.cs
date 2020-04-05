using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public Sprite sprite;
    public MeshRenderer symbolQuad;
    public MeshRenderer floorIndicator;

    public Color selectedColor;
    private Color nonSelectedColor;

    // Start is called before the first frame update
    void Start()
    {
        symbolQuad.material.mainTexture = sprite.texture;
        nonSelectedColor = floorIndicator.material.color;
    }

    public void SelectAsDestination()
    {
        floorIndicator.material.color = selectedColor;
    }

    public void DeselectAsDestination()
    {
        floorIndicator.material.color = nonSelectedColor;
    }
}
