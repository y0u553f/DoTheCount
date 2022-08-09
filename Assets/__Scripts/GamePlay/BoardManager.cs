using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour 
{
    //private enum Direction {  up , down , left , right};
    
    public SquareStateManager[] squaresArray;
    Dictionary<Vector2, SquareStateManager> squares;

    public SquareStateObject[] squareStates;
    public static Dictionary<SquareStateenum, SquareStateObject> states;
    private SquareStateManager currentSquare;
    private int requiredNumber;
    private int currentNumber=0;
    private bool freezInput=false;
    [SerializeField]
    private Animator anim;
    public  void Awake()
    {
        
        squares = new Dictionary<Vector2, SquareStateManager>();
        states = new Dictionary<SquareStateenum, SquareStateObject>();

        try
        {
            for (int i = 0; i < squaresArray.Length; i++)
                squares.Add(squaresArray[i].coordinate, squaresArray[i]);
            for (int i = 0; i < squareStates.Length; i++)
                states.Add(squareStates[i].state, squareStates[i]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    private void OnEnable()
    {
        SetUpPanel();
        SetInitialCurrentSquare();

    }

    private void Update()
    {
        ProcessInput();
    }
  
    private void ProcessInput()
    {
        if (freezInput)
            return;
        //coordinate are sorted from the top left corner to the right bottom 
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToSquare(new Vector2(currentSquare.coordinate.x, currentSquare.coordinate.y + 1));
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToSquare(new Vector2(currentSquare.coordinate.x, currentSquare.coordinate.y - 1));
            return;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToSquare(new Vector2(currentSquare.coordinate.x+1, currentSquare.coordinate.y ));
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToSquare(new Vector2(currentSquare.coordinate.x - 1, currentSquare.coordinate.y));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            anim.SetTrigger("Shake");
            SetUpPanel();
            return;
        }

    }

    private void MoveToSquare(Vector2 targetSquareCoord)
    {
        //this condition works only if the puzzle shape is square (x==y) and coordinate startes with 1 not 0
        if (targetSquareCoord.x < 0 || targetSquareCoord.x > squaresArray.Length || targetSquareCoord.y < 0 || targetSquareCoord.y > squaresArray.Length)
            return;
        SquareStateManager square;
        if (squares.TryGetValue(targetSquareCoord,out square))
        {
            if (square.currentState.canAccess)
            {
                square.SwitchStates(square.Used);
                currentSquare = square;     
                if (square.init == SquareStateenum.endPoint)
                {
                    freezInput = true;
                    CalculateResult();
                    SetUpPanel();
                }
                else
                {
                    currentNumber += currentSquare.Value;

                }
            }
        }
    }


    private void SetInitialCurrentSquare()
    {
        bool entryPointFound=false, endPointFound=false;
        foreach (KeyValuePair<Vector2, SquareStateManager> square in squares)
        {
            if (square.Value.init == SquareStateenum.entryPoint)
            {
                currentSquare = square.Value;
                entryPointFound = true;
                print("square.Value = " + square.Key);
            }
            else if((square.Value.init == SquareStateenum.endPoint))
            {
                requiredNumber = square.Value.Value;
                endPointFound = true;
                
            }
            if (endPointFound && entryPointFound)
                break;
        }
    }

    private void CalculateResult()
    {
        Debug.Log("Result : " + currentNumber + " " + requiredNumber);
        if (currentNumber == requiredNumber)
        {
            Win();  
        }
        else
        {
            Loose();
            
        }
    }

    private void Win()
    {
        AudioManager.instance.PlaySound("correct");
        GameManager.instance.ActivateNextBoard();
    }

    private void Loose()
    {
        anim.SetTrigger("Shake");
        AudioManager.instance.PlaySound("wrong");
    }
   private void SetUpPanel()
    {
        foreach(KeyValuePair<Vector2, SquareStateManager> square in squares)
        {
            square.Value.InitSquare();
            
        }
        SetInitialCurrentSquare();
        freezInput = false;
        currentNumber = 0;
    }
}
