using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject[] locations;
    [SerializeField] GameObject[] spawnedObjects;
    [SerializeField] GameObject cross;
    [SerializeField] GameObject nought;
    [SerializeField] GameObject winLine;
    [SerializeField] Material redMaterial;

    private int tempstartIndex;
    private int tempendIndex;


    public void Start() {
        spawnedObjects = new GameObject[9];
    }

    public void SetBoardState(int[] boardState)
    {
        for (int i = 0; i < 9; i++) // delete all objects
        {
            if (spawnedObjects[i] != null) Destroy(spawnedObjects[i]);
        }

        for (int i = 0; i < 9; i++) // spawn all objects
        {
            switch (boardState[i])
            {
            case 0: //instantiates 0 at location i
                spawnedObjects[i] = Instantiate(nought, locations[i].transform.position, Quaternion.identity, null);
                break;
            case 1: //instantiates X at location i
                spawnedObjects[i] = Instantiate(cross, locations[i].transform.position, Quaternion.identity, null);
                break;
            default: //NOTHING at location i
                break;
            }
        }
    }
    public void CreateWinLineRenderer()
    {
        LineRenderer WinLineRenderer = winLine.GetComponent<LineRenderer>();
        WinLineRenderer.positionCount = 2;
        WinLineRenderer.startWidth = 0.2f;
        WinLineRenderer.endWidth = 0.2f;
        WinLineRenderer.material = redMaterial;
    }
    public void SetWinLineRenderer(int startIndex, int endIndex, bool iswithdelay)
    {
        tempstartIndex = startIndex;
        tempendIndex = endIndex;
        if (iswithdelay) Invoke("SetWinLineRendererDelayed", 0.1f);
        else SetWinLineRendererDelayed();
    }
    private void SetWinLineRendererDelayed()
    {
        LineRenderer WinLineRenderer = winLine.GetComponent<LineRenderer>();
        WinLineRenderer.SetPosition(0, locations[tempstartIndex].transform.position);
        WinLineRenderer.SetPosition(1, locations[tempendIndex].transform.position);
    }
    
}