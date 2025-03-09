using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look for Stock", story: "[Customer] looks for [NavmeshSlot] at shelf", category: "Action", id: "02257b3239c09847811f1752fcb63332")]
public partial class LookForStockAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Customer;
    [SerializeReference] public BlackboardVariable<NavMeshSlot> NavmeshSlot;
    CustomerShoppingManager customerManager;
    NavMeshAgent agent;

    protected override Status OnStart()
    {
        Customer.Value.TryGetComponent(out customerManager);
        Customer.Value.TryGetComponent(out agent);

        if (customerManager == null) return Status.Failure;
        if (agent == null) return Status.Failure;

        if (customerManager.TryGetShoppingListShelf(out StockShelvesManager shelf))
        {
            if (shelf.TryGetComponent(out NavMeshSlotManager navMeshSlotManager))
            {
                if (navMeshSlotManager.TryGetRandomSlot(out NavMeshSlot slot))
                {
                    NavmeshSlot.Value = slot;
                    return Status.Success;
                }
            }

        }

        return Status.Failure;
    }
}

