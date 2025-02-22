using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsNextSlotFree", story: "If next [navmeshslot] is free", category: "Conditions", id: "3bbdd01c0941debd991437ca74c02657")]
public partial class IsNextSlotFreeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Navmeshslot;

    public override bool IsTrue()
    {
        if (Navmeshslot.Value == null) { return false; }

        if (Navmeshslot.Value.TryGetNextSlot(out NavMeshSlot nextSlot))
        {
            if (!nextSlot.IsOccupied) return true;
        }

        return false;
    }
}
