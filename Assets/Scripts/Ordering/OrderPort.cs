using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class OrderPort : NetworkBehaviour
{
    public Order Order;
    public UI_OrderScreen OrderScreen;

    [SerializeField] OrderPortAcceptor orderPortAcceptor;
    [Space]
    [SerializeField] AudioClip CorrectNoise;
    [SerializeField] AudioClip IncorrectNoise;
    [SerializeField] AudioClip TimeoutNoise;
    [Space]
    public UnityEvent OnOrderAdded;
    public UnityEvent OnOrderCorrect;
    public UnityEvent OnOrderIncorrect;
    public UnityEvent OnOrderTimout;

    private void Awake()
    {
        orderPortAcceptor.Init(this);
    }

    public bool TryAddOrder(Order order)
    {
        if (Order != null) { return false; }

        Order = order;
        OrderScreen.AddOrder(order);
        Order.OnOrderRemoved += (context) => { RemoveAllocatedOrder(); };
        Order.OnTimerFinished += (context) => { OrderTimeout_Rpc(); };

        OnOrderAdded.Invoke();

        return true;
    }

    private void RemoveAllocatedOrder()
    {
        Order = null;
    }

    public void ProcessOrderBox(Contents boxContents)
    {
        if (!IsServer) return;
        if (Order == null) return;

        OrderResponse response = Order.CompareContents(boxContents);

        switch (response.ResponseStatus)
        {
            case (ResponseStatus.Success):
                Debug.Log("Great order!");
                MoneyManager.Instance.AddToQuota(response.Profit + response.Loss);
                Order.OnOrderSucceeded.Invoke(Order);
                OrderManager.Instance.AddTimeBonus(Order, response.Profit);
                OrderCorrect_Rpc();
                break;
            case (ResponseStatus.Failure):
                Order.OnOrderFailed.Invoke(Order);
                OrderIncorrect_Rpc();
                OrderManager.Instance.failedOrder.Value = true;
                break;
        }



        OrderManager.Instance.RemoveOrder_Rpc(Order.OrderId);
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
