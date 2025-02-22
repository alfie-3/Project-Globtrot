using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "AtFrontOfQueue", story: "Is [slot] is the front of the queue", category: "Conditions", id: "eb9bc107fcf08d2a54b75726c69e1505")]
public partial class AtFrontOfQueueCondition : Condition
{
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Slot;

    public override bool IsTrue()
    {
        if (Slot.Value == null) return false;

        return Slot.Value.IsFrontOfQueue();
    }
}
