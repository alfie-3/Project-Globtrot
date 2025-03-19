using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class OrderPort : MonoBehaviour
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

            return true;
        }

        return false;
    }

    public void ProcessOrderBox(Contents boxContents)
    {
        OrderResponse response = orderAllocationList[0].Order.CompareContents(boxContents);

        if (response.Success)
        {
            Debug.Log("Great order!");
        }
        else
        {
            Debug.Log("Bad order!");
        }
    }
}
