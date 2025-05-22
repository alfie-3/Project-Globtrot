using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class UpgradesManager : NetworkBehaviour
{
    public static UpgradesManager Instance { get; private set; }

    public Dictionary<string, Upgrade> UnlockableUpgrades { get; private set; } = new();

    public static Action<Upgrade> OnAddedUpgrade = delegate { };
    public static Action<Upgrade> OnUnlockedUpgrade = delegate { };

    public List<Upgrade> CurrentUpgrades { get; private set; } = new();

    [field: SerializeField] public List<UpgradeEvent> SceneUpgradeEvents { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = null;
        OnAddedUpgrade = delegate { };
        OnUnlockedUpgrade = delegate { };
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
    }

    private void OnEnable()
    {
        GameStateManager.OnDayChanged += (current) => { AddUpgrades(); };
    }

    private void OnDisable()
    {
        GameStateManager.OnDayChanged -= (current) => { AddUpgrades(); };
    }

    public void AddUpgrades()
    {
        DayData dayData = GameStateManager.Instance.GetCurrentDayData();

        if (dayData == null) return;

        foreach (Upgrade upgrade in dayData.AddedUpgrades)
        {
            UnlockableUpgrades.Add(upgrade.UpgradeId, upgrade);
            OnAddedUpgrade.Invoke(upgrade);
        }
    }

    [Rpc(SendTo.Server)]
    public void TryPurchaseUpgrade_Rpc(string upgradeId)
    {
        if (!UnlockableUpgrades.TryGetValue(upgradeId, out Upgrade upgrade)) return;

        if (MoneyManager.Instance.TrySpendChips(upgrade.UpgradeCost))
        {
            UnlockUpgrade_Rpc(upgradeId);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UnlockUpgrade_Rpc(string upgradeId)
    {
        if (!UnlockableUpgrades.TryGetValue(upgradeId, out Upgrade upgrade)) return;

        UnlockableUpgrades.Remove(upgradeId);
        CurrentUpgrades.Add(upgrade);

        OnUnlockedUpgrade.Invoke(upgrade);

        TriggerUpgradeEvent(upgrade);
    }

    private void TriggerUpgradeEvent(Upgrade upgrade)
    {
        UpgradeEvent upgradeEvent = SceneUpgradeEvents.FirstOrDefault(upgradeEvent => upgradeEvent.Upgrade == upgrade);

        if (upgradeEvent.Equals(default)) return;
        if (upgradeEvent.SceneEvent == null) return;

        upgradeEvent.SceneEvent.Invoke();
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        GameStateManager.OnDayChanged -= (current) => { AddUpgrades(); };
    }

    public void UpgradePlayer(PlayerUpgradeData data)
    {
        GlobalPlayerModifiers.UpgradeStat(data);
    }

    public void UpgradeProcessor(ProcessorUpgradeData data)
    {
        GlobalProcessorModifiers.UpgradeStat(data);
    }
}

[System.Serializable]
public struct UpgradeEvent
{
    public Upgrade Upgrade;
    public UnityEvent SceneEvent;
}