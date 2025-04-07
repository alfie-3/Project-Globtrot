using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrderManager : NetworkBehaviour
{
    [SerializeField] List<OrderableList> currentOrderableLists;
    [field: SerializeField] public List<OrderPort> OrderPorts { get; private set; } = new List<OrderPort>();

    public static Dictionary<int, Order> CurrentOrders = new();
    public static Action<Order, int> OnNewOrderAdded;

    //Will always be 0 on clients, used by the server as an ID to distinguish orders
    public static int CurrentOrderID = 0;

    public static OrderManager Instance;
    public int OrderLimit = 2;

    public static Action<float> OnOrderTimersUpdate = delegate { };

    public Vector2 minMaxOrderDelayTime = new(5, 10);

    [Space]

    [SerializeField] float[] PlayerTimeMultipliers = new float[4];


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        GameStateManager.OnDayStateChanged += OnDayStateChange;
    }

    public void OnDayStateChange(DayState state)
    {
        if (state == DayState.Open)
        {
            Invoke(nameof(AddNewRandomOrder), GetRandomDelay());
        }
    }

    private void Update()
    {
        OnOrderTimersUpdate.Invoke(Time.deltaTime);
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

        return PlayerTimeMultipliers[NetworkManager.Singleton.ConnectedClients.Count];
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameStateManager.OnDayStateChanged -= OnDayStateChange;
        OnOrderTimersUpdate = null;
    }
}
