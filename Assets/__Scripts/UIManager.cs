using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : Singleton<UIManager>
{

    [SerializeField]
    private Text boardCounter;
    [SerializeField]
    private Text starCountText;
    [SerializeField]
    private float counterSpeed = 0.2f;

    private void OnEnable()
    {
        GameManager.OnStarCountChanged += UpdateStarUI;
    }

    private void OnDisable()
    {
        GameManager.OnStarCountChanged -= UpdateStarUI;
    }

    public void UpdateBoardCounter(int newCounterValue)
    {
        int currentCount = ParseStringToInt(boardCounter.text);
        boardCounter.DOCounter(currentCount, newCounterValue, counterSpeed);
    }

    private void UpdateStarUI(int newStarCount)
    {
        int currentCount = ParseStringToInt(starCountText.text);
        starCountText.DOCounter(currentCount, newStarCount, counterSpeed);
    }

    private int ParseStringToInt(string text)
    {
        int currentCount;
        if (!int.TryParse(text, out currentCount))
        {
            currentCount = 0;
        }
        return currentCount;
    }
}