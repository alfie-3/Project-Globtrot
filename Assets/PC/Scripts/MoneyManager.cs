using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class MoneyManager : NetworkBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public int startingMoney = 100;  // starting money

    static public Action<int, int> OnMoneyChanged = delegate { };

    private NetworkVariable<int> currentMoney = new();
    public int CurrentMoney => currentMoney.Value;

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
        currentMoney.Value+= startingMoney;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        currentMoney.Value = startingMoney;

        OnMoneyChanged.Invoke(CurrentMoney, CurrentMoney);
    }

    public bool CanAfford(float price)
    {
        return CurrentMoney >= price;
    }

    public void SpendMoney(int amount)
    {
        int prev = CurrentMoney;

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

    public void AddMoney(int amount)
    {
        int prev = CurrentMoney;

        currentMoney.Value += amount;

        OnMoneyChanged.Invoke(prev, CurrentMoney);
    }
}
