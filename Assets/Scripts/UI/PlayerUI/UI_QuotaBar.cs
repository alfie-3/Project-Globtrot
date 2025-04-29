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

        GetComponent<Canvas>().enabled = false;
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
        GetComponent<Canvas>().enabled = state == DayState.Open;

    }

    public void QuotaAchieved()
    {
        fillColour.color = successColour;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(progressBar.transform.DOLocalMoveY(progressBarStartingPos.y + 40, 1).SetEase(Ease.InBounce));
        sequence.Append(text.transform.DOLocalMoveY(textStartingPos.y + 20, 1).SetEase(Ease.OutExpo));
    }

    private void OnDestroy()
    {
        MoneyManager.OnQuotaAchieved -= QuotaAchieved;
        MoneyManager.OnUpdateQuotaTarget -= UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged -= UpdateQuotaAmount;
        GameStateManager.OnDayStateChanged -= ToggleBar;
    }
}
