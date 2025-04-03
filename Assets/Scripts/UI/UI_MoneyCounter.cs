using System;
using TMPro;
using UnityEngine;

public class UI_MoneyCounter : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;

    public void Awake()
    {
        //MoneyManager.OnQuotaAmountChanged += UpdateCounter;
    }

    private void UpdateCounter(double prev, double current)
    {
        text.text = MoneyFormatter.FormatPriceInt(current);
    }

    public void OnDestroy()
    {
        //MoneyManager.OnQuotaAmountChanged -= UpdateCounter;
    }
}
