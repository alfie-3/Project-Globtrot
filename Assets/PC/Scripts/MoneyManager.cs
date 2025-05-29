using System;
using Unity.Netcode;
using UnityEngine;

public class MoneyManager : NetworkBehaviour
{
    public const float ChipsMultiplier = 0.08f;

    public static MoneyManager Instance { get; private set; }

    public NetworkVariable<int> Chips { get; private set; } = new();
    public Action<int> OnChipsChanged = delegate { };

    public NetworkVariable<int> BuildCoins { get; private set; } = new();
    public Action<int> OnBuildCoinsChanged = delegate { };

    public NetworkVariable<int> CurrentQuotaAmount = new();
    public NetworkVariable<int> TimeBonus = new();

    public NetworkVariable<int> CurrentQuotaTarget = new();

    static public Action OnQuotaAchieved = delegate { };

    static public Action<int, int> OnQuotaAmountChanged = delegate { };
    static public Action<int> OnUpdateQuotaTarget = delegate { };

    public bool MetQuota => CurrentQuotaAmount.Value >= CurrentQuotaTarget.Value;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        OnQuotaAmountChanged = delegate { };
        OnUpdateQuotaTarget = delegate { };
        OnQuotaAchieved = delegate { };
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

        GameStateManager.OnResetServer += () => { CurrentQuotaAmount.Value = 0; TimeBonus.Value = 0; };
        GameStateManager.OnDayStateChanged += (value) => { if (value == DayState.Open) SetQuotaTarget(); };
        GameStateManager.OnDayChanged += (value) => { DayData dayData = GameStateManager.Instance.GetCurrentDayData(); if (dayData == null) return; AddBuildCoins(dayData.AddedDayCoins); };

        CurrentQuotaAmount.OnValueChanged += (prev, current) =>
    {
        OnQuotaAmountChanged(prev, current);

        if (CurrentQuotaAmount.Value >= CurrentQuotaTarget.Value)
        {
            OnQuotaAchieved.Invoke();
        }
    };

        CurrentQuotaTarget.OnValueChanged += (prev, current) => { OnUpdateQuotaTarget(current); };

        Chips.OnValueChanged += (prev, current) => { OnChipsChanged(current); };
        BuildCoins.OnValueChanged += (prev, current) => { OnBuildCoinsChanged(current); };
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        OnQuotaAmountChanged.Invoke(CurrentQuotaAmount.Value, CurrentQuotaTarget.Value);
        BuildCoins.Value = 300;
    }

    public int GetTotal()
    {
        int total = 0;
        total += CurrentQuotaAmount.Value;
        total += TimeBonus.Value;
        total += OrderManager.CalculatePerfectBonus(CurrentQuotaAmount.Value);

        return total;
    }

    public void SetQuotaTarget()
    {
        if (!IsServer) return;

        DayData dayData = GameStateManager.Instance.GetLatestDayData();
        if (dayData == null) return;

        CurrentQuotaAmount.Value = 0;

        int dailyQuota = dayData.DailyQuota;

        if (OrderManager.Instance != null)
        {
            dailyQuota = (int)(dailyQuota * OrderManager.Instance.GetMultipliers().QuotaTargetMultiplier);
        }

        SetQuotaTarget(dayData.DailyQuota);
    }

    public void SetQuotaTarget(int target)
    {
        if (!IsServer) return;

        CurrentQuotaTarget.Value = target;
    }

    public void AddTimeBonus(int amount)
    {
        TimeBonus.Value += amount;
    }

    public void AddToQuota(int amount)
    {
        CurrentQuotaAmount.Value += amount;
    }

    public bool CanAfford(double price)
    {
        return true;
    }

    public void SpendMoney(int amount)
    {
        //OnQuotaChanged.Invoke(prev, CurrentMoney);
    }

    [ContextMenu("Add 100 chips")]
    public void Add100Chips()
    {
        AddChips(100);
    }

    public void AddChips(int amount)
    {
        if (!IsServer) return;

        Chips.Value += amount;
    }

    public bool TrySpendChips(int amount)
    {
        if (Chips.Value - amount < 0) return false;

        Chips.Value -= amount;
        return true;
    }

    [ContextMenu("Add 100 BuildCoins")]
    public void Add100BuildCoins()
    {
        AddBuildCoins(100);
    }

    [ContextMenu("Complete quota")]
    public void CompleteQuota()
    {
        AddToQuota(CurrentQuotaTarget.Value);
    }

    public void AddBuildCoins(int amount)
    {
        if (!IsServer) return;

        BuildCoins.Value += amount;
    }

    public bool TrySpendBuildCoins(int amount)
    {
        if (BuildCoins.Value - amount < 0) return false;

        BuildCoins.Value -= amount;
        return true;
    }


    public void AddMoney(int amount)
    {

    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayStateChanged -= (value) => { if (value == DayState.Open) SetQuotaTarget(); };
        GameStateManager.OnResetServer -= () => { CurrentQuotaAmount.Value = 0; TimeBonus.Value = 0; };
        GameStateManager.OnDayChanged -= (value) => { DayData dayData = GameStateManager.Instance.GetCurrentDayData(); if (dayData == null) return; AddBuildCoins(dayData.AddedDayCoins); };
    }
}
