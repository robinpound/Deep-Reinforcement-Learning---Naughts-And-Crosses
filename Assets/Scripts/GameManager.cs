using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIManager uIManager;
    [SerializeField] GameBoard gameBoard;

    NeuralNetwork neuralNetwork;

    bool isPlayerTurn;
    bool isPlayerFirst;
    bool isGameEnded;
    
    int[] boardState;

    private void Start()
    {
        neuralNetwork = new NeuralNetwork(18, 18, 11, 9); // DO NOT TOUCH!
        
        uIManager.CreateNetworkDisplay(neuralNetwork);
        uIManager.UpdateNodes(neuralNetwork);
        uIManager.CreateWeights(neuralNetwork);
        populateNeuralNetworkRandomly(); 

        gameBoard.CreateWinLineRenderer();

        //SET UP GAME:
        isPlayerTurn = true;
        isPlayerFirst = true;
        isGameEnded = false;
        boardState = new int[9];
        ResetBoard();
        uIManager.setAIWinTEXT(neuralNetwork.AIWins);
        uIManager.setPlayerWinTEXT(neuralNetwork.playerWins);
        uIManager.setTurnTEXT(isPlayerTurn);
        StatusNothing();

    } 
    public void populateNeuralNetworkRandomly(){ // RESET ANN
        neuralNetwork.PopulateRandomly();
        uIManager.UpdateNodes(neuralNetwork);
    }
    public void ForwardPropNeuralNetwork(){  // PLAY
        float[] boardStateInputs = new float[18];
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] == 0) boardStateInputs[i] = 1;
            if (boardState[i] == 1) boardStateInputs[i+9] = 1;
        }
        neuralNetwork.SetInputNodes(boardStateInputs);                     
        neuralNetwork.ForwardPropagate();
        uIManager.UpdateNodes(neuralNetwork);
    } 
    public void BackwardPropagateNeuralNetwork(int playerchosenSquare){ // LEARN
        float[] Target = new float[9];              
        for (int i=0;i<Target.Length;i++) 
        {
            if (boardState[i] != 3) Target[i] = -10f;
            else Target[i] = -10f;
        }                  
        Target[playerchosenSquare] = 10f;

        neuralNetwork.BackwardPropagate(Target, 0.002f);
        uIManager.UpdateNodes(neuralNetwork);
    }
    public void DisplayWeightDebug() => uIManager.UpdateWeights(neuralNetwork); //NOT WORKING FUNCTION
    
    public void ResetBoard(){
        for (int i = 0; i < 9; i++){boardState[i] = 3;} // Reset board
        isGameEnded = false;
        if (!isPlayerFirst) StartANNFirstMove();
        gameBoard.SetBoardState(boardState);
        isPlayerTurn = true;
    }
    public void SetBoard(int positionOnBoard, int WhichType){
        boardState[positionOnBoard] = WhichType;
        gameBoard.SetBoardState(boardState);
    }
    public void SetBoardWithDelay(int positionOnBoard, int WhichType){
        boardState[positionOnBoard] = WhichType;
        Invoke("delayedSetBoardState", 0.2f);
    }
    private void delayedSetBoardState()
    {
        gameBoard.SetBoardState(boardState);
        uIManager.setTurnTEXT(true);
    }
    public void StartPlayerTurn(int playerchosenSquare) // called by button
    {
        playerchosenSquare--; // shift for sanity sake
        
        bool chosenValidMove = boardState[playerchosenSquare] == 3;
        
        if (isPlayerTurn && !isGameEnded && chosenValidMove) 
        {
            SetBoard(playerchosenSquare, 0);
            bool PlayerHasWon = checkWin(0);
            if(PlayerHasWon)
            {
                neuralNetwork.playerWins ++;
                uIManager.setPlayerWinTEXT(neuralNetwork.playerWins);
                isGameEnded = true;
                Invoke("RestartGame", 1f);
                BackwardPropagateNeuralNetwork(playerchosenSquare); 
                return;
            }
        }
        if (!isGameEnded && chosenValidMove) StartANNTurn();
    }
    public void StartANNTurn()
    {
        isPlayerTurn = false; //-------
        // DECIDE & PLAY
        ForwardPropNeuralNetwork();
        int ValidIndex = FindMaxValueValidIndex(neuralNetwork.GetOutputNodes());
        if (ValidIndex != -1){
            uIManager.setTurnTEXT(false);
            SetBoardWithDelay(ValidIndex, 1);
            bool AIHasWon = checkWin(1);
            if(AIHasWon)
            {
                neuralNetwork.AIWins ++;
                uIManager.setAIWinTEXT(neuralNetwork.AIWins);
                isGameEnded = true;
                Invoke("RestartGame", 1f);
                BackwardPropagateNeuralNetwork(ValidIndex); 
                return;
            }
        } 
        else 
        {
            isGameEnded = true;
            Invoke("RestartGame", 1f);
            return;
        }
        
        isPlayerTurn = true; 

        bool isSpaceInBoard = false;
        for(int i = 0; i< boardState.Length; i++)
        {
            if(boardState[i] == 3) {
                isSpaceInBoard = true;
            }
        }
        if (!isSpaceInBoard)
        {
            isGameEnded = true;
            Invoke("RestartGame", 1f);
        }
    }
    private void StartANNFirstMove(){
        //PLAY
        isPlayerTurn = false; 
        ForwardPropNeuralNetwork();
        int ValidIndex = FindMaxValueValidIndex(neuralNetwork.GetOutputNodes());
        if (ValidIndex != -1) SetBoardWithDelay(ValidIndex, 1);
    }
    private void RestartGame()
    {
        isPlayerFirst = !isPlayerFirst;
        gameBoard.SetWinLineRenderer(0,0,true);
        ResetBoard();
    }
    private int FindMaxValueValidIndex(float[] array)
    {
        int Index = -1;
        float Value = -1000000000f;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] >= Value && boardState[i] == 3)
            {
                Value = array[i];
                Index = i;
            }
        }
        return Index;
    }

    public void SaveButtonPress()
    {
        uIManager.setStatusTEXT("Saving...");
        neuralNetwork.saveNetwork();
        Invoke("StatusNothing", 2f);
    }
    
    public void LoadButtonPress(){
        uIManager.setStatusTEXT("Loading...");
        neuralNetwork.LoadNetwork();
        uIManager.setAIWinTEXT(neuralNetwork.AIWins);
        uIManager.setPlayerWinTEXT(neuralNetwork.playerWins);
        Invoke("StatusNothing", 2f);
    }
    public void StatusNothing() => uIManager.setStatusTEXT("");
    
    public bool checkWin(int player) {
        
        // Check rows
        for (int i = 0; i < 9; i += 3) {
            if (boardState[i] == player && boardState[i + 1] == player && boardState[i + 2] == player) {
                gameBoard.SetWinLineRenderer(i, i+2,true);
                return true;
            }
        }

        // Check columns
        for (int i = 0; i < 3; i++) {
            if (boardState[i] == player && boardState[i + 3] == player && boardState[i + 6] == player) {
                gameBoard.SetWinLineRenderer(i, i+6,true);
                return true;
            }
        }

        // Check diagonals
        if (boardState[0] == player && boardState[4] == player && boardState[8] == player) {
            gameBoard.SetWinLineRenderer(0, 8,true);
            return true;
        }
        if (boardState[2] == player && boardState[4] == player && boardState[6] == player) {
            gameBoard.SetWinLineRenderer(2, 6,true);
            return true;
        }

        return false;
    }
}