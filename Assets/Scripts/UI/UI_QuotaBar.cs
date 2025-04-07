using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuotaBar : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] TextMeshProUGUI text;

    int currentTarget = 0;

    public void Awake()
    {
        MoneyManager.OnUpdateQuotaTarget += UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged += UpdateQuotaAmount;
        GameStateManager.OnDayStateChanged += ToggleBar;

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

    private void OnDestroy()
    {
        MoneyManager.OnUpdateQuotaTarget -= UpdateQuotaTarget;
        MoneyManager.OnQuotaAmountChanged -= UpdateQuotaAmount;
        GameStateManager.OnDayStateChanged -= ToggleBar;
    }
}
