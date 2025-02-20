using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeItem", story: "[Customer] takes item from shelf", category: "Action", id: "b33c8a2731a74217f8716d8a47a537e2")]
public partial class TakeItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Customer;
    private BasicCustomer customerManager;

    protected override Status OnStart()
    {

        if (!Customer.Value.TryGetComponent(out customerManager)) return Status.Failure;

        customerManager.RemoveItemFromShoppingList();

        return Status.Success;
    }
}

