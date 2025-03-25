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
    [Space]
    [SerializeField] AudioClip CorrectNoise;
    [SerializeField] AudioClip IncorrectNoise;
    [SerializeField] AudioClip TimeoutNoise;

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
            allocation.Order.OnTimerFinished += (context) => { PlayNoise_Rpc(ResponseStatus.Timeout); };

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
        if (orderAllocationList.Count == 0) return;

        OrderResponse response = orderAllocationList[0].Order.CompareContents(boxContents);

        switch (response.ResponseStatus)
        {
            case (ResponseStatus.Success):
                Debug.Log("Great order!");
                orderAllocationList[0].Order.OnOrderSucceeded.Invoke(orderAllocationList[0].Order);
                PlayNoise_Rpc(ResponseStatus.Success);
                break;
            case (ResponseStatus.Failure):
                Debug.Log("Bad order!");
                orderAllocationList[0].Order.OnOrderFailed.Invoke(orderAllocationList[0].Order);
                PlayNoise_Rpc(ResponseStatus.Failure);
                break;
        }

        OrderManager.Instance.RemoveOrder_Rpc(orderAllocationList[0].Order.OrderId);
        OrderManager.Instance.AddNewRandomOrder();
    }

    [Rpc(SendTo.Everyone)]
    public void PlayNoise_Rpc(ResponseStatus status)
    {
        switch (status)
        {
            case ResponseStatus.Success:
                GetComponent<AudioSource>().PlayOneShot(CorrectNoise);
                break;
            case ResponseStatus.Failure:
                GetComponent<AudioSource>().PlayOneShot(IncorrectNoise);
                break;
            case ResponseStatus.Timeout:
                GetComponent<AudioSource>().PlayOneShot(TimeoutNoise);
                break;
        }
    }
}
