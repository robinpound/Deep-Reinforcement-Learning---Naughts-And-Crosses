using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public void SaveWeightsAndBiases(float[,] Layer1Weight, float[,] Layer2Weight, float[,] Layer3Weight, float[] Layer1Bias, float[] Layer2Bias, float[] Layer3Bias, int playerWins, int AIWins)
    {
        PlayerPrefs.SetString("Layer1Weight", ArrayToStringMatrix(Layer1Weight));
        PlayerPrefs.SetString("Layer2Weight", ArrayToStringMatrix(Layer2Weight));
        PlayerPrefs.SetString("Layer3Weight", ArrayToStringMatrix(Layer3Weight));

        PlayerPrefs.SetString("Layer1Bias", ArrayToString(Layer1Bias));
        PlayerPrefs.SetString("Layer2Bias", ArrayToString(Layer2Bias));
        PlayerPrefs.SetString("Layer3Bias", ArrayToString(Layer3Bias));

        PlayerPrefs.SetString("playerWins", IntToString(playerWins));
        PlayerPrefs.SetString("AIWins", IntToString(AIWins));

        PlayerPrefs.Save();
    }

    public float[,] LoadWeight(int WeightLayer, int NumberOfColsinLayer) // NumberOfColsinLayer is a GetLength(1)
    {
        string name = "";
        if (WeightLayer == 1) name = "Layer1Weight";
        if (WeightLayer == 2) name = "Layer2Weight";
        if (WeightLayer == 3) name = "Layer3Weight";
        return StringToArrayMatrix(PlayerPrefs.GetString(name), NumberOfColsinLayer);
    }
    public float[] LoadBias(int BiasLayer)
    {
        string biasName = "";
        if (BiasLayer == 1) biasName = "Layer1Bias";
        if (BiasLayer == 2) biasName = "Layer2Bias";
        if (BiasLayer == 3) biasName = "Layer3Bias";
        return StringToArray(PlayerPrefs.GetString(biasName));
    }
    public int LoadWINS(bool isPlayerWins)
    {
        string biasName = "";
        if (isPlayerWins == true) biasName = "playerWins";
        if (isPlayerWins == false) biasName = "AIWins";
        
        return StringToInteger(PlayerPrefs.GetString(biasName));
    }

    private string ArrayToStringMatrix(float[,] array)
    {
        string arrayString = "";
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                arrayString += array[i, j].ToString() + ",";
            }
        }

        return arrayString;
    }
    private string ArrayToString(float[] array)
    {
        return string.Join(",", array);
    }
    private string IntToString(int number)
    {
        return number.ToString();
    }


    private float[,] StringToArrayMatrix(string arrayString, int NumberOfColsinLayer)
    {
        string[] elements = arrayString.Split(',');
        int cols = NumberOfColsinLayer;
        int rows = elements.Length / cols;
        
        float[,] array = new float[rows, cols];

        int index = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = float.Parse(elements[index]);
                index++;
            }
        }

        return array;
    }
    private float[] StringToArray(string arrayString)
    {
        string[] elements = arrayString.Split(',');
        float[] array = new float[elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            array[i] = float.Parse(elements[i]);
        }

        return array;
    }
    private int StringToInteger(string arrayString)
    {
        return int.Parse(arrayString);;
    }
}
