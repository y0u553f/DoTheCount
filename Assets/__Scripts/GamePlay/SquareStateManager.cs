using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SquareStateManager : MonoBehaviour
{
    
    public SquareBaseState currentState;
    public SquareEntryPoint EntryPoint { get; private set; }
    public SquareEndPoint EndPoint { get; private set; }
    public  SquareNormal Normal { get; private set; }
    public SquareUsed Used {  get; private set; }
    public SquareStateenum init;
    //coordinate starts from 1 not 0 , be careful !!
    public Vector2 coordinate;
    public Image placeHolder;
    
    [SerializeField]
    private Text text;
    [Range (0,20)]
    [SerializeField]
    private int value;
    public int Value { get => value;  }
    

  

    public void InitSquare()
    {
        initiStates();
        ShowValueOnText();
        SetInitialSquareState();
        currentState.EnterState(this);
        ShowValueOnText();
    }
    private void SetInitialSquareState()
    {
        switch (init)
        {
            case SquareStateenum.entryPoint:currentState = EntryPoint;
                break;
            case SquareStateenum.normal:currentState = Normal;
                break;
            case SquareStateenum.endPoint:  currentState = EndPoint;
            
                break;
            default:break;
        }
    }

    private void initiStates()
    {
        EntryPoint = new SquareEntryPoint(GetStateObject(SquareStateenum.entryPoint), false);
        EndPoint = new SquareEndPoint(GetStateObject(SquareStateenum.endPoint), true);
        Normal = new SquareNormal(GetStateObject(SquareStateenum.normal), true);
        Used = new SquareUsed(GetStateObject(SquareStateenum.used), false);
    }

    private SquareStateObject GetStateObject(SquareStateenum stateenum )
    {
        SquareStateObject stateobject;
        if (BoardManager.states.TryGetValue(stateenum, out stateobject))
            return stateobject;
        else
            return null;
    }

    public void SwitchStates(SquareBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void ShowValueOnText()
    {
        if (value == 0)
        {
            text.enabled = false;
            return;
        }

        text.text = value.ToString();
    }
}
