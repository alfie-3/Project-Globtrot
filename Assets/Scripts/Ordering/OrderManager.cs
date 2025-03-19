using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Netcode;

public class OrderManager : NetworkBehaviour
{
    [SerializeField] CurrentOrderables currentOrderables;
    [field: SerializeField] public List<OrderPort> OrderPorts { get; private set; } = new List<OrderPort>();

    public static List<Order> CurrentOrders;
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

    private void Start()
    {
        AddNewOrder();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        CurrentOrders = new List<Order>();
        Instance = null;
        OnNewOrderAdded = delegate { };
    }

    public void AddNewOrder()
    {
        if (CurrentOrders.Count >= OrderLimit) { return; }

        CurrentOrders.Add(OrderBuilder.GenerateOrder(currentOrderables));
        OnNewOrderAdded.Invoke(CurrentOrders[^1], CurrentOrders.Count);

        AssignToOrderPort(CurrentOrders[^1]);
    }

    public void RemoveOrder(Order order)
    {
        order.OnOrderRemoved.Invoke(order);
        CurrentOrders.Remove(order);
    }

    public void AssignToOrderPort(Order order)
    {
        foreach (OrderPort port in OrderPorts)
        {
            if (port.TryAddOrder(order)) return;
        }
    }
}
