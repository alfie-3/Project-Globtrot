using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakeItem", story: "[Customer] takes item from [Shelf]", category: "Action", id: "b33c8a2731a74217f8716d8a47a537e2")]
public partial class TakeItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Customer;
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Shelf;

    private CustomerShoppingManager customerManager;

    protected override Status OnStart()
    {

        if (!Customer.Value.TryGetComponent(out customerManager)) return Status.Failure;

        if(Shelf.Value.SlotManager == null) return Status.Failure;

        if (Shelf.Value.SlotManager.TryGetComponent(out StockShelvesManager shelvesManager))
        {
            customerManager.TakeShoppingListItemsFromShelf(shelvesManager);
            return Status.Success;
        }

        return Status.Failure;
    }
}

