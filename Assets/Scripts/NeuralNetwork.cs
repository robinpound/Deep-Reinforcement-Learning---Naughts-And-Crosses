using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public float[] Layer0Nodes; // Input
    public float[] Layer1Nodes;
    public float[] Layer2Nodes;
    public float[] Layer3Nodes; // Output
    
    public float[,] Layer1Weight;
    public float[,] Layer2Weight;
    public float[,] Layer3Weight;

    public float[] Layer1Bias;
    public float[] Layer2Bias;
    public float[] Layer3Bias;

    public int playerWins = 0;
    public int AIWins = 0;


    SaveLoadManager SLM;
    
    public NeuralNetwork(int Num_L0Nodes, int Num_L1Nodes, int Num_L2Nodes, int Num_L3Nodes)
    {
        Layer0Nodes = new float[Num_L0Nodes];
        Layer1Nodes = new float[Num_L1Nodes];
        Layer2Nodes = new float[Num_L2Nodes];
        Layer3Nodes = new float[Num_L3Nodes];

        Layer1Weight = new float[Num_L1Nodes, Num_L0Nodes];
        Layer2Weight = new float[Num_L2Nodes, Num_L1Nodes];
        Layer3Weight = new float[Num_L3Nodes, Num_L2Nodes];

        Layer1Bias = new float[Num_L1Nodes];
        Layer2Bias = new float[Num_L2Nodes];
        Layer3Bias = new float[Num_L3Nodes];
    }
    public void Start() {
        SLM = new SaveLoadManager();
    }

    public void saveNetwork()
    {
        if (SLM == null) SLM = new SaveLoadManager();
        SLM.SaveWeightsAndBiases(Layer1Weight, Layer2Weight, Layer3Weight, Layer1Bias, Layer2Bias, Layer3Bias, playerWins, AIWins);
        Debug.Log("SAVING playerWins:" + playerWins);
        Debug.Log("SAVING AIWins:" + AIWins);
    }
    public void LoadNetwork()
    {
        if (SLM == null) SLM = new SaveLoadManager();
        Layer1Weight = SLM.LoadWeight(1, Layer1Weight.GetLength(1));
        Layer2Weight = SLM.LoadWeight(2, Layer2Weight.GetLength(1));
        Layer3Weight = SLM.LoadWeight(3, Layer3Weight.GetLength(1));

        Layer1Bias = SLM.LoadBias(1);
        Layer2Bias = SLM.LoadBias(2);
        Layer3Bias = SLM.LoadBias(3);

        playerWins = SLM.LoadWINS(true);
        AIWins = SLM.LoadWINS(false);

        Debug.Log("LOADING playerWins:" + playerWins);
        Debug.Log("LOADING AIWins:" + AIWins);
    }

    public void PopulateRandomly()
    {
        populateNodes();
        populateWeights();
        populateBiases();                           
    }

    private void populateNodes()
    {
        for (int i = 0; i < Layer0Nodes.Length; i++) {
            Layer0Nodes[i] = 0;
        }
        for (int i = 0; i < Layer1Nodes.Length; i++) {
            Layer1Nodes[i] = 0;
        }
        for (int i = 0; i < Layer2Nodes.Length; i++) {
            Layer2Nodes[i] = 0;
        }
        for (int i = 0; i < Layer3Nodes.Length; i++) {
            Layer3Nodes[i] = 0;
        }
    }
    private void populateWeights()
    {
        for (int i = 0; i < Layer1Weight.GetLength(0); i++) {
            for (int j = 0; j < Layer1Weight.GetLength(1) ; j ++) {
                Layer1Weight[i,j] = Random.Range(-0.1f, 0.1f);
            }
        }
        for (int i = 0; i < Layer2Weight.GetLength(0); i++) {
            for (int j = 0; j < Layer2Weight.GetLength(1) ; j ++) {
                Layer2Weight[i,j] = Random.Range(-0.1f, 0.1f);
            }
        }
        for (int i = 0; i < Layer3Weight.GetLength(0); i++) {
            for (int j = 0; j < Layer3Weight.GetLength(1) ; j ++) {
                Layer3Weight[i,j] = Random.Range(-0.1f, 0.1f);
            }
        }
    }
    private void populateBiases()
    {
        for (int i = 0; i < Layer1Bias.Length; i++) {
            Layer1Bias[i] = Random.Range(-0.1f, 0.1f);
        }
        for (int i = 0; i < Layer2Bias.Length; i++) {
            Layer2Bias[i] = Random.Range(-0.1f, 0.1f);
        }
        for (int i = 0; i < Layer3Bias.Length; i++) {
            Layer3Bias[i] = Random.Range(-0.1f, 0.1f);
        }
    }

    public void SetInputNodes(float[] inputNodes) => Layer0Nodes = inputNodes;

    public void ForwardPropagate()
    {   
        // Layer 0 => layer 1
        CalculateLayerOutput(Layer0Nodes, Layer1Weight, Layer1Bias, Layer1Nodes); 
        RectifiedLinearUnits(Layer1Nodes);

        // Layer 1 => layer 2
        CalculateLayerOutput(Layer1Nodes, Layer2Weight, Layer2Bias, Layer2Nodes); 
        RectifiedLinearUnits(Layer2Nodes);

        // Layer 2 => layer 3
        CalculateLayerOutput(Layer2Nodes, Layer3Weight, Layer3Bias, Layer3Nodes); 
        //SoftMax(Layer3Nodes);
    }

    private void RectifiedLinearUnits(float[] Nodes)
    {  
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (Nodes[i] < 0) Nodes[i] = 0;
        }
    }
    private void SoftMax(float[] Nodes)
    {
        float sum = 0;
        for (int i = 0; i < Nodes.Length; i++)
        {
            sum += Mathf.Exp(Nodes[i]);
        }
        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = Mathf.Exp(Nodes[i]) / sum;
        }
    }
    private void CalculateLayerOutput(float[] previousLayerNodes, float[,] weights, float[] biases, float[] currentLayerNodes)
    {
        for (int j = 0; j < currentLayerNodes.Length; j++)
        {
            float sum = 0;
            for (int i = 0; i < previousLayerNodes.Length; i++)
            {
                sum += weights[j, i] * previousLayerNodes[i];
            }
            currentLayerNodes[j] = sum + biases[j];
        }
    }

    public float[] GetSoftMaxOfThirdLayer()
    {
        float[] thirdLayerCopy = new float[Layer3Nodes.Length];
        for (int i = 0; i < thirdLayerCopy.Length; i++)
        {
            thirdLayerCopy[i] = Layer3Nodes[i];
        }
        
        float sum = 0;
        for (int i = 0; i < thirdLayerCopy.Length; i++)
        {
            sum += Mathf.Exp(thirdLayerCopy[i]);
        }
        for (int i = 0; i < thirdLayerCopy.Length; i++)
        {
            thirdLayerCopy[i] = Mathf.Exp(thirdLayerCopy[i]) / sum;
        }
        
        return thirdLayerCopy;
    }
    public float[] GetOutputNodes() => Layer3Nodes;

    public void BackwardPropagate(float[] targetOutput, float learningRate)
    {
        float[] layer3Delta = new float[Layer3Nodes.Length];

        // Calculate the error and delta for the output layer
        for (int i = 0; i < Layer3Nodes.Length; i++)
        {
            layer3Delta[i] = targetOutput[i] - Layer3Nodes[i]; // Linear activation
        }

        // Update weights and biases for Layer 2 => Layer 3
        UpdateWeightsAndBiases(Layer2Nodes, layer3Delta, Layer3Weight, Layer3Bias, learningRate);

        // Calculate deltas for Layer 2 (ReLU derivative)
        float[] layer2Delta = CalculateDeltaReLU(layer3Delta, Layer3Weight, Layer3Nodes);

        // Update weights and biases for Layer 1 => Layer 2
        UpdateWeightsAndBiases(Layer1Nodes, layer2Delta, Layer2Weight, Layer2Bias, learningRate);

        // Calculate deltas for Layer 1 (ReLU derivative)
        float[] layer1Delta = CalculateDeltaReLU(layer2Delta, Layer2Weight, Layer2Nodes);

        // Update weights and biases for Layer 0 => Layer 1
        UpdateWeightsAndBiases(Layer0Nodes, layer1Delta, Layer1Weight, Layer1Bias, learningRate);
    }

    private float[] CalculateDeltaReLU(float[] thisLayerDelta, float[,] thisLayerWeight, float[] thisLayerNodes)
    {
        float[] delta = new float[thisLayerWeight.GetLength(1)];

        for (int i = 0; i < thisLayerWeight.GetLength(1); i++)
        {
            float weightedDeltaSum = 0f;
            
            for (int j = 0; j < thisLayerWeight.GetLength(0); j++)
            {
                if (thisLayerNodes[j] > 0f)
                {
                    weightedDeltaSum += thisLayerWeight[j, i] * thisLayerDelta[j];
                }
			}

            delta[i] = weightedDeltaSum;
        }
        return delta;
    }

    private void UpdateWeightsAndBiases(float[] inputNodes, float[] delta, float[,] weights, float[] biases, float learningRate)
    {
        for (int i = 0; i < delta.Length; i++)
        {
            for (int j = 0; j < inputNodes.Length; j++)
            {
                weights[i, j] += learningRate * delta[i] * inputNodes[j];
            }
            biases[i] += learningRate * delta[i];
        }
    }
}