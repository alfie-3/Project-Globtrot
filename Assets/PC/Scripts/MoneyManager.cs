using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private float startingMoney = 100.0f;  // set the starting amount

    private float currentMoney;

    private void Awake()
    {
        // singleton stuff
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    public bool CanAfford(float price)
    {
        return currentMoney >= price;
    }

    public void SpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            currentMoney -= amount;
            UpdateMoneyUI();
        }
        else
        {
            Debug.LogWarning("Not enough money!");
        }
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = $"${currentMoney:F2}";
    }
}
