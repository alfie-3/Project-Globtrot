using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "OccupyNavmeshSlot", story: "[navmeshslot] is [occupied] by [self]", category: "Action", id: "f0ca7bc2ac649038333c0d58f9276987")]
public partial class OccupyNavmeshSlotAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Navmeshslot;
    [SerializeReference] public BlackboardVariable<bool> Occupied;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    NavMeshSlot slot;
    protected override Status OnStart()
    {
        Navmeshslot.Value.TryGetComponent(out slot);

        if (slot == null) return Status.Failure;

        if (Occupied.Value == true)
        {
            slot.Occupy(Self.Value);

            return Status.Success;
        }
        else
        {
            slot.UnOccupy(Self.Value);

            return Status.Success;
        }
    }
}

