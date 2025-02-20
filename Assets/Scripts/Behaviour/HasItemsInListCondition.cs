using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasItemsInList", story: "[customer] has items in their list", category: "Customer", id: "cc4ef398c2799eae13d993e2025c5398")]
public partial class HasItemsInListCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Customer;

    private BasicCustomer customerManager;

    public override bool IsTrue()
    {
        Customer.Value.TryGetComponent(out customerManager);

        if (customerManager == null) return false;

        if (customerManager.HasItemsInShoppingList) return true;

        return false;
    }
}

