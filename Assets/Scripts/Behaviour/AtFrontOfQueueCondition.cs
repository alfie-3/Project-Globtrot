using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "AtFrontOfQueue", story: "Is [slot] is the front of the queue", category: "Conditions", id: "eb9bc107fcf08d2a54b75726c69e1505")]
public partial class AtFrontOfQueueCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Slot;
    NavMeshSlot slot;

    public override bool IsTrue()
    {
        if (slot == null) return false;

        return slot.IsFrontOfQueue();
    }

    public override void OnStart()
    {
        Slot.Value.TryGetComponent(out slot);   
    }

    public override void OnEnd()
    {
    }
}
