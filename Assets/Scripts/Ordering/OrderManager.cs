using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using Unity.Collections;

public class OrderManager : NetworkBehaviour
{
    [SerializeField] CurrentOrderables currentOrderables;
    [field: SerializeField] public List<OrderPort> OrderPorts { get; private set; } = new List<OrderPort>();

    public static Dictionary<FixedString64Bytes, Order> CurrentOrders = new();
    public static Action<Order, int> OnNewOrderAdded;

    public static OrderManager Instance;

    public int OrderLimit = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        AddNewRandomOrder();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        CurrentOrders = new();
        Instance = null;
        OnNewOrderAdded = delegate { };
    }

    public void AddNewRandomOrder()
    {
        if (!IsServer) return;

        if (CurrentOrders.Count >= OrderLimit) { return; }

        Guid uniqueId = Guid.NewGuid();
        Debug.Log(uniqueId.ToString());
        Order newOrder = OrderBuilder.GenerateOrder(currentOrderables, uniqueId.ToString());

        CurrentOrders.Add(uniqueId.ToString(), newOrder);

        OnNewOrderAdded.Invoke(newOrder, CurrentOrders.Count);

        int assignedPort = TryAssignToOrderPort(newOrder);

        SyncOrder_Rpc(new(newOrder, assignedPort));
    }

    public void AddNewOrderFromPayload(OrderPayload payload)
    {
        Order order = new(payload.Time, payload.OrderItemsToList(), payload.OrderID);

        CurrentOrders.Add(payload.OrderID, order);
        AssignToOrderPort(order, payload.AssignedPort);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SyncOrder_Rpc(OrderPayload orderPayload)
    {
        if (IsServer) return;

        AddNewOrderFromPayload(orderPayload);
    }

    [Rpc(SendTo.Everyone)]
    public void RemoveOrder_Rpc(FixedString64Bytes orderId)
    {
        if (CurrentOrders.TryGetValue(orderId, out Order order))
        {
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
}
