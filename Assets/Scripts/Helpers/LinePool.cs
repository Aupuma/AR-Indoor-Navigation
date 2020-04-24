using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePool : MonoBehaviour
{
    [SerializeField] LineRenderer _arLinePrefab;
    [SerializeField] GameObject _cornerPrefab;
    [SerializeField] int _amountToPool;

    List<LineRenderer> _linePool;
    List<GameObject> _cornerPool;

    float _lineHeight = 0;
    public float LineHeight { get => _lineHeight; set => _lineHeight = value; }

    // Start is called before the first frame update
    void Start()
    {
        _linePool = new List<LineRenderer>();
        _cornerPool = new List<GameObject>();
        for (int i = 0; i < _amountToPool; i++)
        {
            AddLine();
            AddCorner();
        }

        AddCorner(); //End corner
    }

    private void AddCorner()
    {
        if (_cornerPrefab != null)
        {
            GameObject endCorner = Instantiate(_cornerPrefab, this.transform);
            endCorner.SetActive(false);
            _cornerPool.Add(endCorner);
        }
    }

    private void AddLine()
    {
        LineRenderer line = Instantiate(_arLinePrefab, this.transform);
        line.gameObject.SetActive(false);
        _linePool.Add(line);
    }

    public void SetLinePositions(Vector3[] positionsArray)
    {
        HideLines(); //Just to make sure previous lines don't stay visible

        if (_cornerPrefab != null) //Initial corner
        {
            Vector3 startCornerPos = new Vector3(positionsArray[0].x, positionsArray[0].y + _lineHeight, positionsArray[0].z);
            _cornerPool[0].transform.position = startCornerPos;
            _cornerPool[0].SetActive(true);
        }

        for (int i = 0; i < positionsArray.Length - 1; i++)
        {
            if(i > _linePool.Count)
            {
                AddLine();
                AddCorner();
            }

            Vector3 posA = new Vector3(positionsArray[i].x, positionsArray[i].y + _lineHeight, positionsArray[i].z);
            _linePool[i].SetPosition(0, posA);

            Vector3 posB = new Vector3(positionsArray[i + 1].x, positionsArray[i + 1].y + _lineHeight, positionsArray[i + 1].z);
            _linePool[i].SetPosition(1, posB);

            _linePool[i].gameObject.SetActive(true);

            if (_cornerPrefab != null) //Initial corner
            {
                _cornerPool[i + 1].transform.position = posB;
                _cornerPool[i + 1].SetActive(true);
            }
        }
    }

    public void HideLines()
    {
        foreach (var item in _linePool)
        {
            item.gameObject.SetActive(false);
        }

        if (_cornerPrefab != null)
            foreach (var item in _cornerPool)
            {
                item.SetActive(false);
            }
    }
}
