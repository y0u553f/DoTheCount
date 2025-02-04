using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    [SerializeField]
    private Text boardCounter;

    public void UpdateBoardCounter(string text)
    {
        boardCounter.text = text;
    }
}
