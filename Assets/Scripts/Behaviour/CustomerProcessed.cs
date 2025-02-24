using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/CustomerProcessed ")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "CustomerProcessed ", message: "Customer is processed", category: "Customer", id: "faba025f6ce3b6cf1659f0d0226ee3b7")]
public partial class CustomerProcessed : EventChannelBase
{
    public delegate void CustomerProcessedEventHandler();
    public event CustomerProcessedEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        CustomerProcessedEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as CustomerProcessedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as CustomerProcessedEventHandler;
    }
}

