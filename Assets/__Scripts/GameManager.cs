using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int currentBoard = 0;
    [SerializeField]
    private GameObject[] boards;
    //Be careful all boards should have the tag "Boards";
    private const string BOARDSTAG= "Boards";
    //Last Message to show to the player when they finish the game
    [SerializeField]
    private GameObject gameOverText;
  

    public void ActivateNextBoard()
    {
        boards[currentBoard].SetActive(false);
        currentBoard++;
        if (boards.Length > currentBoard)
            boards[currentBoard].SetActive(true);
        else
            gameOverText.SetActive(true);
    }
}
