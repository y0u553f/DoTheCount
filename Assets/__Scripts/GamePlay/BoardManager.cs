using MobileHapticsProFreeEdition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private SquareStateManager[] squaresArray;
    [SerializeField]
    private SquareStateObject[] squareStates;
    [SerializeField]
    private SquareStateManager[] squaresSolution;
    [Header("Reset Settings")]
    [SerializeField, Tooltip("Delay before starting square reset after shake animation")]
    private float shakeAnimationDelay = 0.1f;

    [SerializeField, Tooltip("Time between individual square resets")]
    private float squareResetInterval = 0.2f;

    [SerializeField, Tooltip("Time to play color animation of number  fornhints")]
    private float hintAnimationInterval = 0.8f;



    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private EventSystem eventSystem;

    private Dictionary<Vector2, SquareStateManager> squares;
    public static Dictionary<SquareStateenum, SquareStateObject> states;
    private SquareStateManager currentSquare;
    private Stack<SquareStateManager> usedSquaresStack;
    private int requiredNumber;
    private int currentNumber = 0;
    private bool freezInput = false;
    private PointerEventData pointerEventData;


    public void Awake()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        squares = new Dictionary<Vector2, SquareStateManager>();
        states = new Dictionary<SquareStateenum, SquareStateObject>();
        usedSquaresStack = new Stack<SquareStateManager>();

        try
        {
            foreach (var square in squaresArray)
                squares.Add(square.coordinate, square);

            foreach (var state in squareStates)
                states.Add(state.state, state);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Initialization error: {e.Message}");
        }
    }


    public void ShowHint()
    {
        foreach (SquareStateManager square in squaresSolution)
        {
            square.HighlightSquare(hintAnimationInterval);

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
                    if (usedSquaresStack.Count > 0)
                        StartCoroutine(ResetBoard());
                    break;
            }
        }
    }

    private void HandleUITouch(Vector2 touchPosition)
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = touchPosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            SquareStateManager square = result.gameObject.GetComponent<SquareStateManager>();
            if (square != null && square != currentSquare)
            {
                TryMoveToSquare(square.coordinate, square);
                break;
            }
        }
    }

    private IEnumerator ResetBoard()
    {

        TapticWave.TriggerHaptic(HapticModes.Failure);

        freezInput = true;
        anim.SetTrigger("Shake");
        yield return ResetUsedSquaresCoroutine();

        SetUpPanel();
    }

    private IEnumerator ResetUsedSquaresCoroutine()
    {
        freezInput = true;
        yield return new WaitForSeconds(shakeAnimationDelay);

        while (usedSquaresStack.Count > 0)
        {
            SquareStateManager square = usedSquaresStack.Pop();
            if (square.init != SquareStateenum.endPoint)
            {
                print("print state :" + square.currentState.state.state);
                UpdateCurrentNumber(currentNumber - square.Value);
            }
            square.SwitchStates(square.Normal);

            yield return new WaitForSeconds(squareResetInterval);
            if (usedSquaresStack.Count == 0)
                yield return new WaitForSeconds(square.currentState.state.transitionDuration);

        }

    }


    private void TryMoveToSquare(Vector2 targetSquareCoord, SquareStateManager square)
    {
        if (Mathf.Abs(targetSquareCoord.x - currentSquare.coordinate.x) +
            Mathf.Abs(targetSquareCoord.y - currentSquare.coordinate.y) != 1)
        {
            StartCoroutine(ResetBoard());
            return;
        }

        if (square.currentState == square.Used)
        {
            StartCoroutine(ResetBoard());
            return;
        }

        MoveToSquare(targetSquareCoord);
        TapticWave.TriggerHaptic(HapticModes.Confirm);
    }

    private void MoveToSquare(Vector2 targetSquareCoord)
    {
        if (targetSquareCoord.x < 0 || targetSquareCoord.x > squaresArray.Length ||
            targetSquareCoord.y < 0 || targetSquareCoord.y > squaresArray.Length)
            return;

        if (squares.TryGetValue(targetSquareCoord, out SquareStateManager square))
        {
            if (square.currentState.canAccess)
            {

                if (square.init == SquareStateenum.endPoint)
                {
                    freezInput = true;
                    CalculateResult();
                    //SetUpPanel();
                }
                else
                {
                    square.SwitchStates(square.Used);
                    currentSquare = square;
                    usedSquaresStack.Push(square);
                    UpdateCurrentNumber(currentNumber + currentSquare.Value);
                }
            }
        }
    }

    private void SetInitialCurrentSquare()
    {
        bool entryFound = false, endFound = false;
        foreach (var square in squares)
        {
            if (square.Value.init == SquareStateenum.entryPoint)
            {
                currentSquare = square.Value;
                entryFound = true;
            }
            else if (square.Value.init == SquareStateenum.endPoint)
            {
                requiredNumber = square.Value.Value;
                endFound = true;
            }

            if (entryFound && endFound) break;
        }
    }

    private void CalculateResult()
    {
        if (currentNumber == requiredNumber) Win();
        else Loose();
    }

    private void Win()
    {
        AudioManager.instance.PlaySound("correct");
        GameManager.instance.ActivateNextBoard();
    }

    private void Loose()
    {
        AudioManager.instance.PlaySound("wrong");
        StartCoroutine(ResetBoard());
    }

    private void SetUpPanel()
    {
        foreach (var square in squares)
        {
            square.Value.InitSquare();
        }
        SetInitialCurrentSquare();
        freezInput = false;
        UpdateCurrentNumber(0);
        usedSquaresStack.Clear();
    }

    private void UpdateCurrentNumber(int value)
    {
        currentNumber = value;
        UIManager.instance.UpdateBoardCounter(currentNumber);
    }
}