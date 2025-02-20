using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetNextSlotInQueue", story: "[Self] looks for next [slot] in queue", category: "Action", id: "57c61eda8a6d8c64a0d41ef5c2636cf9")]
public partial class GetNextSlotInQueueAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Slot;

    protected override Status OnStart()
    {
        if (Slot.Value.TryGetComponent(out NavMeshSlot slot))
        {
            if (slot.TryGetNextSlot(out NavMeshSlot nextSlot))
            {
                Slot.Value = nextSlot.gameObject;
                return Status.Success;
            }
        }

        return Status.Failure;
    }
}

