using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ReturnToRoaming")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ReturnToRoaming", message: "Return to roaming", category: "Events", id: "d422f810c94425314d9b07ede28afca6")]
public partial class ReturnToRoaming : EventChannelBase
{
    public delegate void ReturnToRoamingEventHandler();
    public event ReturnToRoamingEventHandler Event; 

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
        ReturnToRoamingEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as ReturnToRoamingEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as ReturnToRoamingEventHandler;
    }
}

