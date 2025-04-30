using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Profiling;

public class OrderManager : NetworkBehaviour
{
    [SerializeField] List<OrderableList> currentOrderableLists;
    [field: SerializeField] public List<OrderPort> OrderPorts { get; private set; } = new List<OrderPort>();

    public static Dictionary<int, Order> CurrentOrders = new();
    public static int CurrentOrdersAmount => CurrentOrders.Count;

    public static Action<Order, int> OnNewOrderAdded;
    public static Action OnOrderRemoved;

    //Will always be 0 on clients, used by the server as an ID to distinguish orders
    public static int CurrentOrderID = 0;

    public static OrderManager Instance;
    public int OrderLimit = 2;

    public static Action<float> OnOrderTimersUpdate = delegate { };

    public Vector2 minMaxOrderDelayTime = new(5, 10);

    [Space]

    [SerializeField] float[] PlayerTimeMultipliers = new float[4];
    [Space]
    [SerializeField] Vector2 minMaxSpeedLimits = new(0.25f, 0.5f);
    [SerializeField] Vector2 minMaxSpeedMultipliers = new(1.05f, 1.25f);


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        GameStateManager.OnDayStateChanged += OnDayStateChange;
        GameStateManager.OnDayChanged += UpdateOrderablesList;
    }

    public void UpdateOrderablesList(int day)
    {
        DayData dayData = GameStateManager.Instance.GetLatestDayData();
        if (dayData == null) return;

        currentOrderableLists = dayData.OrderableLists;

        foreach (OrderableList list in currentOrderableLists)
        {
            list.Init();
        }
    }

    public void OnDayStateChange(DayState state)
    {
        if (state == DayState.Open)
        {
            for (int i = 0; i < OrderPorts.Count; i++)
            {
                Invoke(nameof(AddNewRandomOrder), GetRandomDelay() * (i + 1));
            }
        }
    }

    private void Update()
    {
        OnOrderTimersUpdate?.Invoke(Time.deltaTime);
    }

    public float GetRandomDelay()
    {
        return UnityEngine.Random.Range(minMaxOrderDelayTime.x, minMaxOrderDelayTime.y);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        CurrentOrders = new();
        Instance = null;
        OnNewOrderAdded = delegate { };
        OnOrderRemoved = delegate { };
        OnOrderTimersUpdate = delegate { };
        CurrentOrderID = 0;
    }

    public void AddNewRandomOrder()
    {
        if (!IsServer) return;
        if (GameStateManager.Instance.CurrentDayState.Value != DayState.Open) return;
        if (CurrentOrders.Count >= OrderLimit) { return; }

        CurrentOrderID++;


        float multiplier = GetTimeMultiplier();
        Order newOrder = OrderBuilder.GenerateOrder(currentOrderableLists, CurrentOrderID, multiplier);

        AddOrder(newOrder);

        int assignedPort = TryAssignToOrderPort(newOrder);

        if (assignedPort == -1) return;

        SyncOrder_Rpc(new(newOrder, assignedPort), NetworkManager.LocalTime.TimeAsFloat);
    }

    public void AddOrder(Order newOrder)
    {
        CurrentOrders.Add(newOrder.OrderId, newOrder);
        OnNewOrderAdded.Invoke(newOrder, CurrentOrders.Count);
        OnOrderTimersUpdate += newOrder.UpdateTimer;

        if (!IsServer) return;
        newOrder.OnTimerFinished += TimeoutOrder;
    }

    public Order AddNewOrderFromPayload(OrderPayload payload, float sentTime = 0)
    {
        Order order = new(payload.Time, payload.OrderItemsToList(), payload.OrderID);
        AddOrder(order);

        AssignToOrderPort(order, payload.AssignedPort);

        return order;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SyncOrder_Rpc(OrderPayload orderPayload, float sentTime = 0)
    {
        if (IsServer) return;

        Order newOrder = AddNewOrderFromPayload(orderPayload);
        
        if (sentTime != 0)
        {
            newOrder.CurrentOrderTime -= sentTime - NetworkManager.ServerTime.TimeAsFloat;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveOrder_Rpc(int orderId)
    {

        if (CurrentOrders.TryGetValue(orderId, out Order order))
        {
            OnOrderTimersUpdate -= order.UpdateTimer;
            order.OnOrderRemoved.Invoke(order);
        }

        CurrentOrders.Remove(orderId);
        OnOrderRemoved.Invoke();
    }

    public int TryAssignToOrderPort(Order order)
    {
        for (int i = 0; i < OrderPorts.Count; i++)
        {
            if (OrderPorts[i].TryAddOrder(order)) return i;
        }

        return -1;
    }

    public void AssignToOrderPort(Order order, int portIndex)
    {
        OrderPorts[portIndex].TryAddOrder(order);
    }

    public void TimeoutOrder(Order order)
    {
        OrderResponse orderResponse = new(ResponseStatus.Timeout);

        Debug.Log("Out of time!");
        order.OnOrderFailed.Invoke(order);

        RemoveOrder_Rpc(order.OrderId);
        Invoke(nameof(AddNewRandomOrder), GetRandomDelay());
    }

    public float GetTimeMultiplier()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > PlayerTimeMultipliers.Length)
        {
            return 1;
        }

        return PlayerTimeMultipliers[NetworkManager.Singleton.ConnectedClients.Count - 1];
    }

    public void AddTimeBonus(Order order, float profits)
    {
        float normalisedCompletionTime = order.CurrentOrderTime / order.InitialOrderTime;

        if (normalisedCompletionTime < minMaxSpeedLimits.x) { return; }

        normalisedCompletionTime = Mathf.Clamp(normalisedCompletionTime, minMaxSpeedLimits.x, minMaxSpeedLimits.y);
        float multiplier = math.remap(minMaxSpeedLimits.x, minMaxSpeedLimits.y, minMaxSpeedMultipliers.x, minMaxSpeedMultipliers.y, normalisedCompletionTime);

        profits *= multiplier;

        MoneyManager.Instance?.AddTimeBonus((int)profits);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameStateManager.OnDayChanged -= UpdateOrderablesList;
        GameStateManager.OnDayStateChanged -= OnDayStateChange;
        OnOrderTimersUpdate = null;
    }
}

[System.Serializable]
public struct SpeedBonus
{
    float normalizedTime;
    float multiplier;
}