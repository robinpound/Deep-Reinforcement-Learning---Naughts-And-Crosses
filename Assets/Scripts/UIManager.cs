using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject NodeCircleText;
    [SerializeField] GameObject JustText;

    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;

    GameObject[] Layer0NodeCircleText;
    GameObject[] Layer1NodeCircleText;
    GameObject[] Layer2NodeCircleText;
    GameObject[] Layer3NodeCircleText;
    GameObject[] Layer3NodePercentText;

    LineRenderer[] Weight1LineRenderers;
    LineRenderer[] Weight2LineRenderers;
    LineRenderer[] Weight3LineRenderers;

    [SerializeField] TextMeshProUGUI playerWinsTEXT;
    [SerializeField] TextMeshProUGUI AIWinsTEXT;
    [SerializeField] TextMeshProUGUI StatusTEXT;
    [SerializeField] TextMeshProUGUI TurnTEXT;

    public void CreateNetworkDisplay(NeuralNetwork neuralNetwork)
    {
        Layer0NodeCircleText = CreateNodes(neuralNetwork.Layer0Nodes, transform.position, 1.1f, 0f, 0f, false);
        Layer1NodeCircleText = CreateNodes(neuralNetwork.Layer1Nodes, transform.position, 1.1f, 5f, 0f, false);
        Layer2NodeCircleText = CreateNodes(neuralNetwork.Layer2Nodes, transform.position, 1.1f, 10f, -4f, false);
        Layer3NodeCircleText = CreateNodes(neuralNetwork.Layer3Nodes, transform.position, 1.1f, 15f, -5f, false);
        Layer3NodePercentText = CreateNodes(neuralNetwork.Layer3Nodes, transform.position, 1.1f, 16.5f, -5f, true);
    }
    private GameObject[] CreateNodes(float[] nodes, Vector2 startingPoint, float yspacing, float xoffset, float yoffset, bool isPecent)
    {
        Vector2 location = startingPoint + new Vector2(xoffset,yoffset);
        GameObject[] NodeCTList = new GameObject[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            if(isPecent) NodeCTList[i] = Instantiate(JustText , location, Quaternion.identity);
            else {NodeCTList[i] = Instantiate(NodeCircleText , location, Quaternion.identity);}
            location += new Vector2(0,-yspacing);
        }
        return NodeCTList;
    }
    public void UpdateNodes(NeuralNetwork neuralNetwork)
    {
        UpdateNodeLayer(Layer0NodeCircleText, neuralNetwork.Layer0Nodes);
        UpdateNodeLayer(Layer1NodeCircleText, neuralNetwork.Layer1Nodes);
        UpdateNodeLayer(Layer2NodeCircleText, neuralNetwork.Layer2Nodes);
        UpdateNodeLayer(Layer3NodeCircleText, neuralNetwork.Layer3Nodes);
        UpdatePercentLayer(Layer3NodePercentText, neuralNetwork);
    }

    private void UpdateNodeLayer(GameObject[] nodesLayerObjects, float[] nodeLayer)
    {
        for (int i = 0; i < nodesLayerObjects.Length; i++)
        {
            TextMeshProUGUI textComponent = nodesLayerObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = nodeLayer[i].ToString("0.00");
        }
    }

    private void UpdatePercentLayer(GameObject[] NodePercentTextLayerObjects, NeuralNetwork neuralNetwork)
    {
        float[] softmaxedlayer = neuralNetwork.GetSoftMaxOfThirdLayer();
        for (int i = 0; i < NodePercentTextLayerObjects.Length; i++)
        {
            TextMeshProUGUI textComponent = NodePercentTextLayerObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = softmaxedlayer[i].ToString("0%");
        }
    }
    public void CreateWeights(NeuralNetwork neuralNetwork){
        for (int i = 0; i < Layer1NodeCircleText.Length; i++) 
        {
            GameObject startPoint = Layer1NodeCircleText[i];
            if (Weight1LineRenderers == null) Weight1LineRenderers = new LineRenderer[neuralNetwork.Layer1Weight.GetLength(0)];
            Weight1LineRenderers[i] = startPoint.AddComponent<LineRenderer>();
            
            // Set Line Renderer properties that are common for all connections from this startPoint
            Weight1LineRenderers[i].positionCount = Layer0NodeCircleText.Length * 2; // Double the count for start and end positions
            Weight1LineRenderers[i].startWidth = 0.05f;
            Weight1LineRenderers[i].endWidth = 0.05f;
            Weight1LineRenderers[i].material = redMaterial;

            for (int j = 0; j < Layer0NodeCircleText.Length; j++) 
            {
                GameObject endPoint = Layer0NodeCircleText[j];
                int vertexIndex = j * 2; // Two positions per connection
                Weight1LineRenderers[i].SetPosition(vertexIndex, startPoint.transform.position);
                Weight1LineRenderers[i].SetPosition(vertexIndex + 1, endPoint.transform.position);
            }
        }
        for (int i = 0; i < Layer2NodeCircleText.Length; i++) 
        {
            GameObject startPoint = Layer2NodeCircleText[i];
            if (Weight2LineRenderers == null) Weight2LineRenderers = new LineRenderer[neuralNetwork.Layer2Weight.GetLength(0)];
            Weight2LineRenderers[i] = startPoint.AddComponent<LineRenderer>();
            
            // Set Line Renderer properties that are common for all connections from this startPoint
            Weight2LineRenderers[i].positionCount = Layer1NodeCircleText.Length * 2; // Double the count for start and end positions
            Weight2LineRenderers[i].startWidth = 0.05f;
            Weight2LineRenderers[i].endWidth = 0.05f;
            Weight2LineRenderers[i].material = redMaterial;

            for (int j = 0; j < Layer1NodeCircleText.Length; j++) 
            {
                GameObject endPoint = Layer1NodeCircleText[j];
                int vertexIndex = j * 2; // Two positions per connection
                Weight2LineRenderers[i].SetPosition(vertexIndex, startPoint.transform.position);
                Weight2LineRenderers[i].SetPosition(vertexIndex + 1, endPoint.transform.position);
            }
        }
        for (int i = 0; i < Layer3NodeCircleText.Length; i++) 
        {
            GameObject startPoint = Layer3NodeCircleText[i];
            if (Weight3LineRenderers == null) Weight3LineRenderers = new LineRenderer[neuralNetwork.Layer3Weight.GetLength(0)];
            Weight3LineRenderers[i] = startPoint.AddComponent<LineRenderer>();
            
            // Set Line Renderer properties that are common for all connections from this startPoint
            Weight3LineRenderers[i].positionCount = Layer2NodeCircleText.Length * 2; // Double the count for start and end positions
            Weight3LineRenderers[i].startWidth = 0.05f;
            Weight3LineRenderers[i].endWidth = 0.05f;
            Weight3LineRenderers[i].material = redMaterial;

            for (int j = 0; j < Layer2NodeCircleText.Length; j++) 
            {
                GameObject endPoint = Layer2NodeCircleText[j];
                int vertexIndex = j * 2; // Two positions per connection
                Weight3LineRenderers[i].SetPosition(vertexIndex, startPoint.transform.position);
                Weight3LineRenderers[i].SetPosition(vertexIndex + 1, endPoint.transform.position);
            }
        }
    }
    private void SetColours (LineRenderer[] WLR, float[,] WeightLayer)
    {
        for (int i = 0; i < WLR.Length; i++) // for every node
        {
            // THIS IS NOT PROPERLY IMPLEMENTED, BUT IT NEEDS TO BE
            int maxIndex = FindMaxIndex(WeightLayer,i); 
            var mats = new List<Material>();
            for (int ms = 0; ms < 18; ms++) //populate
            {
                if (ms == 0) mats.Add(greenMaterial);
                else mats.Add(redMaterial);
            }
            
            
            
            WLR[i].SetMaterials(mats); // THIS DOESN'T WORK!!!
        }

    }
    
    public void UpdateWeights(NeuralNetwork neuralNetwork)
    { 
        //Weight1LineRenderers[0].material = greenMaterial;
        
        SetColours(Weight1LineRenderers, neuralNetwork.Layer1Weight);
    }

    int FindMaxIndex(float[,] weightLayer, int NodeIndex)
    {
        int maxIndex = 0;
        float maxValue = weightLayer[NodeIndex, 0];

        for (int i = 1; i < weightLayer.GetLength(1); i++)
        {
            if (weightLayer[NodeIndex, i] > maxValue)
            {
                maxValue = weightLayer[NodeIndex, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }
    public void setPlayerWinTEXT(int n) => playerWinsTEXT.text = n.ToString();
    public void setAIWinTEXT(int n) => AIWinsTEXT.text = n.ToString();
    public void setStatusTEXT(string s) => StatusTEXT.text = s;
    public void setTurnTEXT(bool b)
    {
        if (b) TurnTEXT.text = "PLAYER";
        else TurnTEXT.text = "VERONICA";
    }
}