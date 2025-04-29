using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
    public NetworkVariable<int> CurrentDay = new();
    public static Action<int> OnDayChanged = delegate { };

    public NetworkVariable<DayState> CurrentDayState = new();
    public bool IsOpen => CurrentDayState.Value == DayState.Open;
    public static Action<DayState> OnDayStateChanged = delegate { };
    public static Action StartWorkingDay = delegate { };
    [Space]
    [SerializeField, Range(0.01f, 1f)] float timeSpeed = 1f; 
    public NetworkVariable<int> CurrentGameTime = new();
    public static Action<int> OnGameTimeChanged = delegate { };
    bool timerRunning;
    float timer;


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
        CurrentDay.OnValueChanged += (prev, current) => OnDayChanged.Invoke(current);
        CurrentGameTime.OnValueChanged += (prev, current) => OnGameTimeChanged.Invoke(current);

        if (!IsServer) return;

        CurrentDayState.Value = DayState.Preperation;
        mainScene = SceneManager.GetActiveScene();

        CurrentDay.Value = 0;
    }

    [Rpc(SendTo.Server)]
    public void BeginDay_Rpc()
    {
        if (CurrentDayState.Value != DayState.Preperation) return;

        CurrentGameTime.Value = 540;
        ToggleTimer(true);

        CurrentDayState.Value = DayState.Open;
    }

    [Rpc(SendTo.Server)]
    public void EndDay_Rpc(bool bypassOpen = false)
    {
        if (CurrentDayState.Value != DayState.Open && bypassOpen != false) return;

        CurrentDayState.Value = DayState.Closed;
        ToggleTimer(false);

        DayEnd();
    }

    [ContextMenu("SkipDay")]
    public void SkipDay()
    {
        EndDay_Rpc();
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

    public void ToggleTimer(bool on)
    {
        timer = 0;
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
        timer += Time.deltaTime;

        if (timer > timeSpeed)
        {
            CurrentGameTime.Value++;
            timer = 0;
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
    }
}

public enum DayState
{
    Preperation,
    Open,
    Closed
}
