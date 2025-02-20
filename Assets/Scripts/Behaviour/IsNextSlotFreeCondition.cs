using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsNextSlotFree", story: "If next [navmeshslot] is free", category: "Conditions", id: "3bbdd01c0941debd991437ca74c02657")]
public partial class IsNextSlotFreeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Navmeshslot;

    public override bool IsTrue()
    {
        if (Navmeshslot.Value.TryGetComponent(out NavMeshSlot slot))
        {
            if (slot.TryGetNextSlot(out NavMeshSlot nextSlot))
            {
                if (!nextSlot.IsOccupied) return true;
            }
        }

        return false;
    }
}
