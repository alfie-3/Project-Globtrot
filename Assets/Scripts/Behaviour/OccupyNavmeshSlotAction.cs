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
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Navmeshslot;
    [SerializeReference] public BlackboardVariable<bool> Occupied;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        if (Navmeshslot.Value == null) return Status.Failure;

        if (Occupied.Value == true)
        {
            Navmeshslot.Value.Occupy(Self.Value);

            return Status.Success;
        }
        else
        {
            Navmeshslot.Value.UnOccupy(Self.Value);

            return Status.Success;
        }
    }
}

