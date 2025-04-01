using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
    public NetworkVariable<int> CurrentDay = new();

    public static Action<bool> OnDayStateChanged = delegate { };

    public static Action OnReset = delegate { };
    public static Action OnResetServer = delegate { };

    public NetworkVariable<DayState> CurrentDayState = new();
    public bool IsOpen => CurrentDayState.Value == DayState.Open;

    public static GameStateManager Instance { get; private set; }

    Scene mainScene;
    Scene dayEndScene;

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
        OnReset = delegate { };
        OnResetServer = delegate { };
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        CurrentDayState.Value = DayState.Preperation;
        mainScene = SceneManager.GetActiveScene();
        CurrentDay.Value = 0;
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

        DayEnd();
    }

    public void DayEnd()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene("DayEndScene", LoadSceneMode.Additive);
        CheckStatus(status);
    }

    public void NewDay()
    {
        if (dayEndScene == null) return;

        var status = NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName("DayEndScene"));
        CheckStatus(status, false);

        CurrentDay.Value++;
        CurrentDayState.Value = DayState.Preperation;

        ResetLevel_Rpc();
    }

    [Rpc(SendTo.Everyone)]
    public void ResetLevel_Rpc()
    {
        OnReset.Invoke();

        if (IsServer)
        {
            OnResetServer.Invoke();
        }
    }

    private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
    {
        var sceneEventAction = isLoading ? "load" : "unload";
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to {sceneEventAction} {dayEndScene} with" +
                $" a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }
}

public enum DayState
{
    Preperation,
    Open,
    Closed
}
