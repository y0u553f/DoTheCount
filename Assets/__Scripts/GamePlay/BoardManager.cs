using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public SquareStateManager[] squaresArray;
    private Dictionary<Vector2, SquareStateManager> squares;

    public SquareStateObject[] squareStates;
    public static Dictionary<SquareStateenum, SquareStateObject> states;

    private SquareStateManager currentSquare;
    private int requiredNumber;
    private int currentNumber = 0;
    private bool freezInput = false;

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GraphicRaycaster raycaster;
    [SerializeField]
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    public void Awake()
    {
        // Initialize dictionaries
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
        ProcessTouchInput();
    }

    private void ProcessTouchInput()
    {
        if (freezInput)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    HandleUITouch(touch.position);
                    break;

                case TouchPhase.Ended:
                    ResetBoard();
                    break;
            }
        }
    }

    private void HandleUITouch(Vector2 touchPosition)
    {
        // Create a new PointerEventData for the current touch position
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = touchPosition
         };

        // Perform a raycast to detect UI elements
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            // Check if the object hit has a SquareStateManager
            SquareStateManager square = result.gameObject.GetComponent<SquareStateManager>();
            if (square != null && square != currentSquare)
            {
                TryMoveToSquare(square.coordinate, square);

                break;
            }
        }
    }

    private void ResetBoard()
    {
        // Reset the board when the touch ends
        anim.SetTrigger("Shake");
        SetUpPanel();
    }

    private void TryMoveToSquare(Vector2 targetSquareCoord, SquareStateManager square)
    {
        // Prevent diagonal moves
        if (Mathf.Abs(targetSquareCoord.x - currentSquare.coordinate.x) + Mathf.Abs(targetSquareCoord.y - currentSquare.coordinate.y) != 1)
        {
            ResetBoard();
            return;
        }

        // Prevent revisiting already active squares
        if (square.currentState == square.Used)
        {
            ResetBoard();
            return;
        }


        MoveToSquare(targetSquareCoord);
    }

    private void MoveToSquare(Vector2 targetSquareCoord)
    {
        if (targetSquareCoord.x < 0 || targetSquareCoord.x > squaresArray.Length || targetSquareCoord.y < 0 || targetSquareCoord.y > squaresArray.Length)
            return;

        SquareStateManager square;
        if (squares.TryGetValue(targetSquareCoord, out square))
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
                    UpdateCurrentNumber(currentNumber + currentSquare.Value);
                }
            }
        }
    }

    private void SetInitialCurrentSquare()
    {
        bool entryPointFound = false, endPointFound = false;
        foreach (KeyValuePair<Vector2, SquareStateManager> square in squares)
        {
            if (square.Value.init == SquareStateenum.entryPoint)
            {
                currentSquare = square.Value;
                entryPointFound = true;
            }
            else if ((square.Value.init == SquareStateenum.endPoint))
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
        foreach (KeyValuePair<Vector2, SquareStateManager> square in squares)
        {
            square.Value.InitSquare();
        }
        SetInitialCurrentSquare();
        freezInput = false;
        UpdateCurrentNumber(0);

    }

    private void UpdateCurrentNumber(int value)
    {
        currentNumber = value;
        UIManager.instance.UpdateBoardCounter(currentNumber.ToString());
    }
}
