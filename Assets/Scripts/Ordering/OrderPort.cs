using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] List<OrderAllocation> orderAllocationList = new();
    [Space]
    [SerializeField] AudioClip CorrectNoise;
    [SerializeField] AudioClip IncorrectNoise;
    [SerializeField] AudioClip TimeoutNoise;
    [Space]
    public UnityEvent OnOrderCorrect;
    public UnityEvent OnOrderIncorrect;
    public UnityEvent OnOrderTimout;

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
            allocation.Order.OnTimerFinished += (context) => { OrderTimeout_Rpc(); };

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
        if (orderAllocationList[0] == null) return;

        OrderResponse response = orderAllocationList[0].Order.CompareContents(boxContents);

        switch (response.ResponseStatus)
        {
            case (ResponseStatus.Success):
                Debug.Log("Great order!");
                MoneyManager.Instance.AddToQuota(response.Profit + response.Loss);
                orderAllocationList[0].Order.OnOrderSucceeded.Invoke(orderAllocationList[0].Order);
                OrderCorrect_Rpc();
                break;
            case (ResponseStatus.Failure):
                orderAllocationList[0].Order.OnOrderFailed.Invoke(orderAllocationList[0].Order);
                OrderIncorrect_Rpc();
                break;
        }

        OrderManager.Instance.RemoveOrder_Rpc(orderAllocationList[0].Order.OrderId);
        OrderManager.Instance.Invoke(nameof(OrderManager.AddNewRandomOrder), OrderManager.Instance.GetRandomDelay());
    }

    [Rpc(SendTo.Everyone)]
    public void OrderCorrect_Rpc()
    {
        GetComponent<AudioSource>().PlayOneShot(CorrectNoise);
        OnOrderCorrect.Invoke();

    }

    [Rpc(SendTo.Everyone)]
    public void OrderIncorrect_Rpc()
    {
        GetComponent<AudioSource>().PlayOneShot(IncorrectNoise);
        OnOrderIncorrect.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    public void OrderTimeout_Rpc()
    {
        GetComponent<AudioSource>().PlayOneShot(TimeoutNoise);
        OnOrderTimout.Invoke();
    }
}
