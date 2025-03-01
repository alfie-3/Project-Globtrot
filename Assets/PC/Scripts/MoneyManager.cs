using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class MoneyManager : NetworkBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public double startingMoney = 100;  // starting money

    static public Action<double, double> OnMoneyChanged = delegate { };

    private NetworkVariable<double> currentMoney = new();
    public double CurrentMoney => currentMoney.Value;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        OnMoneyChanged = delegate { };
    }

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

        currentMoney.OnValueChanged += (prev, current) => { OnMoneyChanged(prev, current); };
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        currentMoney.Value = startingMoney;

        OnMoneyChanged.Invoke(CurrentMoney, CurrentMoney);
    }

    public bool CanAfford(double price)
    {
        return CurrentMoney >= price;
    }

    public void SpendMoney(double amount)
    {
        double prev = CurrentMoney;

        if (CanAfford(amount))
        {
            currentMoney.Value -= amount;
        }
        else
        {
            Debug.LogWarning("Not enough money!");
        }

        OnMoneyChanged.Invoke(prev, CurrentMoney);
    }

    public void AddMoney(double amount)
    {
        double prev = CurrentMoney;

        currentMoney.Value += amount;

        OnMoneyChanged.Invoke(prev, CurrentMoney);
    }
}
