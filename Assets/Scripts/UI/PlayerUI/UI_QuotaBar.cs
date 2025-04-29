using DG.Tweening;
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

    public void Awake()
    {
        progressBarStartingPos = progressBar.transform.localPosition;
        textStartingPos = text.transform.localPosition;

        defaultColour = fillColour.color;

        MoneyManager.OnUpdateQuotaTarget += UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged += UpdateQuotaAmount;

        GameStateManager.OnDayStateChanged += ToggleBar;
        MoneyManager.OnQuotaAchieved += QuotaAchieved;

        RectTransform rect = transform as RectTransform;
        rect.anchoredPosition = new(rect.anchoredPosition.x, 100);
    }

    public void UpdateQuotaTarget(int newTarget)
    {
        currentTarget = newTarget;
        progressBar.maxValue = currentTarget;
        text.text = $"{0} / {currentTarget}";
    }

    public void UpdateQuotaAmount(int prev, int current)
    {
        progressBar.value = current;
        text.text = $"{current} / {currentTarget}";

    }

    public void ToggleBar(DayState state)
    {
        RectTransform rect = transform as RectTransform;

        if (state == DayState.Open)
        {
            rect.DOAnchorPosY(0.5f, 1).SetEase(Ease.InOutExpo);
        }
        else
        {
            rect.DOAnchorPosY(100, 1).SetEase(Ease.InOutExpo);
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
    }
}
