using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuotaBar : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] Image fillColour;
    [SerializeField] Color successColour = Color.green;
    Color defaultColour;
    [Space]
    [SerializeField] TextMeshProUGUI text;

    int currentTarget = 0;

    Vector3 progressBarStartingPos;
    Vector3 textStartingPos;

    bool barEnabled = false;

    public void Awake()
    {
        progressBarStartingPos = progressBar.transform.localPosition;
        textStartingPos = text.transform.localPosition;

        defaultColour = fillColour.color;

        MoneyManager.OnUpdateQuotaTarget += UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged += UpdateQuotaAmount;

        GameStateManager.OnDayStateChanged += ToggleBar;
        MoneyManager.OnQuotaAchieved += QuotaAchieved;

        GameStateManager.OnReset += ResetBar;

        RectTransform rect = transform as RectTransform;
        rect.anchoredPosition = new(rect.anchoredPosition.x, 100);
    }

    private void ResetBar()
    {
        RectTransform rect = transform as RectTransform;

        fillColour.color = defaultColour;
        text.color = Color.white;

        progressBar.transform.localPosition = progressBarStartingPos;
        text.transform.localPosition = textStartingPos;
    }

    public void UpdateQuotaTarget(int newTarget)
    {
        currentTarget = newTarget;
        progressBar.maxValue = currentTarget;
        text.text = $"<sprite=0>{0} / {currentTarget}";
    }

    public void UpdateQuotaAmount(int prev, int current)
    {
        progressBar.value = current;
        text.text = $"<sprite=0>{current} / {currentTarget}";

    }

    public void ToggleBar(DayState state)
    {
        RectTransform rect = transform as RectTransform;

        if (state == DayState.Open || state == DayState.Overtime)
        {
            if (barEnabled) return;

            rect.DOAnchorPosY(0.5f, 1).SetEase(Ease.InOutExpo);
            barEnabled = true;
        }
        else
        {
            if (!barEnabled) return;

            rect.DOAnchorPosY(100, 1).SetEase(Ease.InOutExpo);
            barEnabled = false;
        }
    }

    public void QuotaAchieved()
    {
        fillColour.color = successColour;
        text.color = successColour;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(progressBar.transform.DOLocalMoveY(progressBarStartingPos.y + 40, 1).SetEase(Ease.InExpo));
        sequence.Append(text.transform.DOLocalMoveY(textStartingPos.y + 10, 1).SetEase(Ease.OutExpo));
    }

    private void OnDestroy()
    {
        MoneyManager.OnQuotaAchieved -= QuotaAchieved;
        MoneyManager.OnUpdateQuotaTarget -= UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged -= UpdateQuotaAmount;
        GameStateManager.OnDayStateChanged -= ToggleBar;
        GameStateManager.OnReset -= ResetBar;
    }
}
