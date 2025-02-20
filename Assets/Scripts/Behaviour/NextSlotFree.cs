using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Next Slot Free")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Next Slot Free", message: "Next slot free", category: "Events", id: "71c7961d63501d1edba74083ed4eec4b")]
public partial class NextSlotFree : EventChannelBase
{
    public delegate void NextSlotFreeEventHandler();
    public event NextSlotFreeEventHandler Event; 

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
        NextSlotFreeEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as NextSlotFreeEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as NextSlotFreeEventHandler;
    }
}

