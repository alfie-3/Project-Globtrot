using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Clock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clockText;

    [SerializeField] float Offset;

    public void OnEnable()
    {
        GameStateManager.OnDayStateChanged += DayStateChanged;
        GameStateManager.OnGameTimeChanged += UpdateText;

        clockText.rectTransform.anchoredPosition = new(Offset, clockText.rectTransform.anchoredPosition.y);
    }

    public void OnDisable()
    {
        GameStateManager.OnGameTimeChanged -= UpdateText;
    }

    public void DayStateChanged(DayState dayState)
    {
        RectTransform rect = clockText.rectTransform;

        if (dayState == DayState.Open)
        {
            rect.DOAnchorPosX(0, 2).SetEase(Ease.OutExpo);
        }
        else
        {
            rect.DOAnchorPosX(Offset, 2);
        }
    }

    public void UpdateText(int currentTime)
    {
        int hours = currentTime / 60;
        int minutes = currentTime % 60;

        clockText.text = $"{hours:00} : {minutes:00}";
    }

    private void OnDestroy()
    {
        GameStateManager.OnDayStateChanged -= DayStateChanged;
        GameStateManager.OnGameTimeChanged -= UpdateText;
    }
}
