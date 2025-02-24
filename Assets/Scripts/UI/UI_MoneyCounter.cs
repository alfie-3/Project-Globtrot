using System;
using TMPro;
using UnityEngine;

public class UI_MoneyCounter : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;

    public void Awake()
    {
        MoneyManager.OnMoneyChanged += UpdateCounter;
    }

    private void UpdateCounter(int prev, int current)
    {
        text.text = current.ToString();
    }

    public void OnDestroy()
    {
        MoneyManager.OnMoneyChanged -= UpdateCounter;
    }
}
