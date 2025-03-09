using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look For Cash Register", story: "[Agent] looks for [Slot] at cash register", category: "Action", id: "8da7aba124800897eff1887ab1b1bb33")]
public partial class LookForCashRegisterAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Slot;
    CustomerShoppingManager customerManager;
    NavMeshAgent agent;

    protected override Status OnStart()
    {
        Agent.Value.TryGetComponent(out customerManager);
        Agent.Value.TryGetComponent(out agent);

        if (customerManager == null) return Status.Failure;
        if (agent == null) return Status.Failure;

        if (CashRegister.TryGetRandomCashRegister(out CashRegister register))
        {
            if (register.Queue == null) { return Status.Failure; }

            if (register.Queue.TryGetFreeSlot(out NavMeshSlot navMeshSlot))
            {
                Slot.Value = navMeshSlot;
                return Status.Success;
            }
        }

        return Status.Failure;
    }
}

