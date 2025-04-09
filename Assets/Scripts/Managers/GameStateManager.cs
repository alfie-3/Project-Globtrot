using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
    public NetworkVariable<int> CurrentDay = new();
    public static Action<int> OnDayChanged = delegate { };

    public static Action<DayState> OnDayStateChanged = delegate { };
    public static Action StartWorkingDay = delegate { };

    public static Action OnReset = delegate { };
    public static Action OnResetServer = delegate { };

    public NetworkVariable<DayState> CurrentDayState = new();
    public bool IsOpen => CurrentDayState.Value == DayState.Open;

    public static GameStateManager Instance { get; private set; }

    [SerializeField] DayDataList DayDataList;

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

        CurrentDayState.OnValueChanged += (prev, current) => { OnDayStateChanged.Invoke(current); };
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        OnDayChanged = delegate { };
        OnDayStateChanged = delegate { };
        OnReset = delegate { };
        OnResetServer = delegate { };
        StartWorkingDay = delegate { };
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnDayChanged.Invoke(0);

        if (!IsServer) return;

        CurrentDayState.Value = DayState.Preperation;
        mainScene = SceneManager.GetActiveScene();

        CurrentDay.OnValueChanged += (prev, current) => OnDayChanged.Invoke(current);
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

    public DayData GetLatestDayData()
    {
        if (DayDataList == null)
        {
            Debug.Log("No day data set");
            return null;
        }

        if (CurrentDay.Value < DayDataList.DayList.Count - 1) return DayDataList.DayList[CurrentDay.Value];
        else
        {
            return DayDataList.DayList[^1];
        }
    }

    public DayData GetCurrentDayData()
    {
        if (CurrentDay.Value > DayDataList.DayList.Count) return null;
        return DayDataList.DayList[CurrentDay.Value];
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

    private new void OnDestroy()
    {
        base.OnDestroy();

        OnDayChanged = delegate { };
        OnDayStateChanged = delegate { };
    }
}

public enum DayState
{
    Preperation,
    Open,
    Closed
}
