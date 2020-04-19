using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePool : MonoBehaviour
{
    [SerializeField] LineRenderer _arLinePrefab;
    [SerializeField] int _amountToPool;
    List<LineRenderer> _linePool;

    // Start is called before the first frame update
    void Start()
    {
        _linePool = new List<LineRenderer>();
        for (int i = 0; i < _amountToPool; i++)
        {
            LineRenderer line = Instantiate(_arLinePrefab, this.transform);
            line.gameObject.SetActive(false);
            _linePool.Add(line);
        }
    }

    public void SetLinePositions(Vector3[] positionsArray)
    {
        HideLines(); //Just to make sure previous lines don't stay visible

        for (int i = 0; i < positionsArray.Length - 1; i++)
        {
            if(i > _linePool.Count)
            {
                LineRenderer line = Instantiate(_arLinePrefab, this.transform);
                line.gameObject.SetActive(false);
                _linePool.Add(line);
            }

            _linePool[i].SetPosition(0, positionsArray[i]);
            _linePool[i].SetPosition(1, positionsArray[i + 1]);
            _linePool[i].gameObject.SetActive(true);
        }
    }

    public void HideLines()
    {
        foreach (var item in _linePool)
        {
            item.gameObject.SetActive(false);
        }
    }
}
