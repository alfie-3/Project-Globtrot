using TMPro;
using UnityEngine;

public class UI_ChipsCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CounterText;

    private void Start()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnChipsChanged += UpdateChipsCounter;

        }
    }

    public void UpdateChipsCounter(int amount)
    {
        CounterText.text = $"<sprite=0> {amount}";
    }
}
