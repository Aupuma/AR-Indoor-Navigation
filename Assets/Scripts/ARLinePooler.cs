using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARLinePooler : MonoBehaviour
{
    public LineRenderer arLinePrefab;
    public int amountToPool;
    private List<LineRenderer> linePool;

    // Start is called before the first frame update
    void Start()
    {
        linePool = new List<LineRenderer>();
        for (int i = 0; i < amountToPool; i++)
        {
            LineRenderer line = Instantiate(arLinePrefab, this.transform);
            line.gameObject.SetActive(false);
            linePool.Add(line);
        }
    }

    public void SetLinePositions(Vector3[] positionsArray)
    {
        HideLines(); //Just to make sure previous lines don't stay visible

        for (int i = 0; i < positionsArray.Length - 1; i++)
        {
            if(i > linePool.Count)
            {
                LineRenderer line = Instantiate(arLinePrefab, this.transform);
                line.gameObject.SetActive(false);
                linePool.Add(line);
            }

            linePool[i].SetPosition(0, positionsArray[i]);
            linePool[i].SetPosition(1, positionsArray[i + 1]);
            linePool[i].gameObject.SetActive(true);
        }
    }

    public void HideLines()
    {
        foreach (var item in linePool)
        {
            item.gameObject.SetActive(false);
        }
    }
}
