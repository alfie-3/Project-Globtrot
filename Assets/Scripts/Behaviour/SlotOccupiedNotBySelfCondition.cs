using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SlotOccupiedNotBySelf", story: "[slot] is occupied not by [self]", category: "Conditions", id: "3ab69c5a93e0e8f132c614dfc5b916bb")]
public partial class SlotOccupiedNotBySelfCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Slot;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        //Abort if target value has been destroyed
        if (Slot.Value == null) return true;

        if (Slot.Value.TryGetComponent(out NavMeshSlot slot))
        {

            //Return false if the slot is not occupied or is occupied by slef - return true if slot is occupied by another agent
            if (slot.IsOccupied)
            {
                if (slot.OccupyingGameObject == null) return false;
                if (slot.OccupyingGameObject == Self.Value)
                {
                    return false;
                }

                return true;
            }
            else return false;
        }

        return true;
    }
}
