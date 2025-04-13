using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using UnityEngine.InputSystem.LowLevel;

public class MoneyManager : NetworkBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public NetworkVariable<int> Chips { get; private set; } = new();
    public Action<int> OnChipsChanged = delegate { };

    public NetworkVariable<int> CurrentQuotaAmount = new();
    public NetworkVariable<int> CurrentQuotaTarget = new();

    static public Action OnQuotaAchieved = delegate { };

    static public Action<int, int> OnQuotaAmountChanged = delegate { };
    static public Action<int> OnUpdateQuotaTarget = delegate { };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        OnQuotaAmountChanged = delegate { };
        OnUpdateQuotaTarget = delegate { };
        OnQuotaAchieved = delegate {};
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

        GameStateManager.OnResetServer += () => { CurrentQuotaAmount.Value = 0; };
        GameStateManager.OnDayStateChanged += (value) => { if (value == DayState.Open) SetQuotaTarget(); };

        CurrentQuotaAmount.OnValueChanged += (prev, current) => { OnQuotaAmountChanged(prev, current); };
        CurrentQuotaTarget.OnValueChanged += (prev, current) => { OnUpdateQuotaTarget(current); };

        Chips.OnValueChanged += (prev, current) => { OnChipsChanged(current); };
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        OnQuotaAmountChanged.Invoke(CurrentQuotaAmount.Value, CurrentQuotaTarget.Value);
    }

    public void SetQuotaTarget()
    {
        DayData dayData = GameStateManager.Instance.GetLatestDayData();
        if (dayData == null) return;

        CurrentQuotaAmount.Value = 0;
        SetQuotaTarget(dayData.DailyQuota);
    }

    public void SetQuotaTarget(int target)
    {
        CurrentQuotaTarget.Value = target;
    }

    public void AddToQuota(int amount)
    {
        CurrentQuotaAmount.Value += amount;

        if (CurrentQuotaAmount.Value >= CurrentQuotaTarget.Value)
        {
            OnQuotaAchieved.Invoke();
            GameStateManager.Instance.EndDay_Rpc();
        }
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

    public void AddMoney(int amount)
    {
        
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayStateChanged -= (value) => { if (value == DayState.Open) SetQuotaTarget(); };
        GameStateManager.OnResetServer -= () => { CurrentQuotaAmount.Value = 0; };
    }
}
