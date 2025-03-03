using System;
using Unity.Netcode;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static Action<bool> OnShopOpenChanged = delegate { };

    public NetworkVariable<ShopState> CurrentShopState = new();
    public bool IsShopOpen => CurrentShopState.Value == ShopState.Open;

    public static GameStateManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        CurrentShopState.OnValueChanged += (current, prev) => { OnShopOpenChanged.Invoke(current != ShopState.Open); };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        OnShopOpenChanged = delegate { };
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        CurrentShopState.Value = ShopState.Preperation;
    }

    [Rpc(SendTo.Server)]
    public void ResetState_Rpc()
    {
        CurrentShopState.Value = ShopState.Preperation;
    }

    [Rpc(SendTo.Server)]
    public void BeginDay_Rpc()
    {
        if (CurrentShopState.Value != ShopState.Preperation) return;

        CurrentShopState.Value = ShopState.Open;
    }

    [Rpc(SendTo.Server)]
    public void EndDay_Rpc()
    {
        if (CurrentShopState.Value != ShopState.Open) return;

        CurrentShopState.Value = ShopState.Closed;
    }
}

public enum ShopState
{
    Preperation,
    Open,
    Closed
}
