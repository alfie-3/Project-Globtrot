using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrderPort : NetworkBehaviour
{
    [System.Serializable]
    private class OrderAllocation
    {
        public bool OrderActive => Order != null;
        public UI_OrderScreen OrderScreen;
        public Order Order;
    }

    [SerializeField] OrderPortAcceptor orderPortAcceptor;
    [SerializeField] List<OrderAllocation> orderAllocationList;

    private void Awake()
    {
        orderPortAcceptor.Init(this);
    }

    public bool TryAddOrder(Order order)
    {
        foreach (OrderAllocation allocation in orderAllocationList)
        {
            if (allocation.OrderActive) { return false; }

            allocation.Order = order;
            allocation.OrderScreen.AddOrder(order);
            allocation.Order.OnOrderRemoved += (context) => { RemoveAllocatedOrder(allocation); };

            return true;
        }

        return false;
    }

    private void RemoveAllocatedOrder(OrderAllocation allocation)
    {
        allocation.Order = null;
    }

    public void ProcessOrderBox(Contents boxContents)
    {
        if (!IsServer) return;

        OrderResponse response = orderAllocationList[0].Order.CompareContents(boxContents);

        if (response.Success)
        {
            Debug.Log("Great order!");
            orderAllocationList[0].Order.OnOrderSucceeded.Invoke(orderAllocationList[0].Order);
        }
        else
        {
            Debug.Log("Bad order!");
            orderAllocationList[0].Order.OnOrderFailed.Invoke(orderAllocationList[0].Order);
        }

        OrderManager.Instance.RemoveOrder_Rpc(orderAllocationList[0].Order.OrderId);
        OrderManager.Instance.AddNewRandomOrder();
    }
}
