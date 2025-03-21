using System;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static Action<bool> OnDayStateChanged = delegate { };

    public NetworkVariable<DayState> CurrentDayState = new();
    public bool IsShopOpen => CurrentDayState.Value == DayState.Open;

    public static GameStateManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        CurrentDayState.OnValueChanged += (current, prev) => { OnDayStateChanged.Invoke(current != DayState.Open); };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        OnDayStateChanged = delegate { };
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        CurrentDayState.Value = DayState.Preperation;
    }

    [Rpc(SendTo.Server)]
    public void ResetState_Rpc()
    {
        CurrentDayState.Value = DayState.Preperation;
    }

    [Rpc(SendTo.Server)]
    public void BeginDay_Rpc()
    {
        if (CurrentDayState.Value != DayState.Preperation) return;

        CurrentDayState.Value = DayState.Open;
    }

    [Rpc(SendTo.Server)]
    public void EndDay_Rpc()
    {
        if (CurrentDayState.Value != DayState.Open) return;

        CurrentDayState.Value = DayState.Closed;
    }
}

public enum DayState
{
    Preperation,
    Open,
    Closed
}
