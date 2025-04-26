using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int starCount = 0;

    public static event Action<int> OnStarCountChanged;

    private int currentBoard = 0;
    [SerializeField]
    private GameObject[] boards;
    //Be careful all boards should have the tag "Boards";
    private const string BOARDSTAG = "Boards";
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

    public void ActivateHintsInActiveBoard()
    {
        boards[currentBoard].GetComponent<BoardManager>().ShowHint();
    }

    private void OnEnable()
    {
        SquareStateManager.OnStarCollected += IncrementStarCount;
    }

    private void OnDisable()
    {
        SquareStateManager.OnStarCollected -= IncrementStarCount;
    }

    private void IncrementStarCount()
    {
        starCount++;
        OnStarCountChanged?.Invoke(starCount);

    }
}