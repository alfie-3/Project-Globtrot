using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
    public NetworkVariable<int> CurrentDay = new();
    public static Action<int> OnDayChanged = delegate { };

    public NetworkVariable<DayState> CurrentDayState = new();

    public bool IsOpen => CurrentDayState.Value == DayState.Open;
    public static Action<DayState> OnDayStateChanged = delegate { };
    public static Action StartWorkingDay = delegate { };
    [field: SerializeField] public List<DayEvent> SceneDayEvents { get; private set; }

    [Space]

    public NetworkVariable<float> CurrentGameTime = new();
    [SerializeField] int dayStartTime = 0;
    [SerializeField] int dayEndTime = 300;
    public static Action<float> OnGameTimeChanged = delegate { };
    bool timerRunning;


    public static Action OnReset = delegate { };
    public static Action OnResetServer = delegate { };


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

    private void Update()
    {
        if (!IsServer) return;

        UpdateTime();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        OnDayChanged = delegate { };
        OnDayStateChanged = delegate { };
        OnGameTimeChanged = delegate {};
        OnReset = delegate { };
        OnResetServer = delegate { };
        StartWorkingDay = delegate { };
        Instance = null;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnDayChanged.Invoke(0);
        CurrentDay.OnValueChanged += (prev, current) => OnDayChanged.Invoke(current);;
        OnDayChanged += (val) => TriggerDayEvent();
        CurrentGameTime.OnValueChanged += (prev, current) => OnGameTimeChanged.Invoke(current / dayEndTime);

        if (!IsServer) return;

        CurrentDayState.Value = DayState.Preperation;
        mainScene = SceneManager.GetActiveScene();

        CurrentDay.Value = 0;
        CurrentGameTime.Value = dayStartTime;
    }

    [Rpc(SendTo.Server)]
    public void BeginDay_Rpc()
    {
        if (CurrentDayState.Value != DayState.Preperation) return;

        if (!RequiredBuildablesManager.HasRequiredBuildables(out RequiredBuildablesResponse response))
        {
            AlertMissingBuildables_Rpc(response.CreateNotification());
            return;
        }

        CurrentGameTime.Value = dayStartTime;
        ToggleTimer(true);

        CurrentDayState.Value = DayState.Open;
    }

    [Rpc(SendTo.Everyone)]
    public void AlertMissingBuildables_Rpc(FixedString64Bytes notification)
    {
        UI_Notifcation.EnqueueNotification(notification.ToString());
    }

    [Rpc(SendTo.Server)]
    public void Close_Rpc(bool bypassOpen = false)
    {
        if (CurrentDayState.Value != DayState.Open && bypassOpen != false) return;

        if (OrderManager.CurrentOrdersAmount > 0)
        {
            CurrentDayState.Value = DayState.Overtime;
            OrderManager.OnOrderRemoved += WaitForOvertimeToFinish;

            return;
        }

        CurrentDayState.Value = DayState.Closed;
        ToggleTimer(false);
    }

    public void WaitForOvertimeToFinish()
    {
        if (OrderManager.CurrentOrdersAmount > 0)
        {
            return;
        }
        else
        {
            Close_Rpc();
            OrderManager.OnOrderRemoved -= WaitForOvertimeToFinish;
        }
    }

    [ContextMenu("SkipDay")]
    public void SkipDay()
    {
        CurrentGameTime.Value = dayEndTime;
    }

    public void EndDay_Rpc()
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

    private void TriggerDayEvent()
    {
        DayData dayData = GetCurrentDayData();
        if (dayData == null) return;

        DayEvent dayEvent = SceneDayEvents.FirstOrDefault(dEvent => dEvent.DayData == dayData);

        if (dayEvent.Equals(default)) return;

        dayEvent.SceneEvent?.Invoke();
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

    public void ToggleTimer(bool on)
    {
        if (on)
        {
            timerRunning = true;
        }
        else
        {
            timerRunning = false;
        }
    }

    public void UpdateTime()
    {
        if (!IsServer || !timerRunning) return;

        CurrentGameTime.Value += Time.deltaTime;

        if (CurrentGameTime.Value >= dayEndTime && CurrentDayState.Value != DayState.Overtime)
        {
            Close_Rpc();
        }
    }

    public DayData GetCurrentDayData()
    {
        if (CurrentDay.Value > DayDataList.DayList.Count - 1) return null;
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
        OrderManager.OnOrderRemoved -= WaitForOvertimeToFinish;
        OnDayChanged -= (val) => TriggerDayEvent();
    }
}

public enum DayState
{
    Preperation,
    Open,
    Overtime,
    Closed
}

[System.Serializable]
public struct DayEvent
{
    public DayData DayData;
    public UnityEvent SceneEvent;
}
