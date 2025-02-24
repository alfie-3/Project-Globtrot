using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "GetNavmeshSlotManager", story: "Get [manager] of [navmeshslot]", category: "Customer", id: "1ccdb82d8feb89f4775cd14a6f35f2ee")]
public partial class GetNavmeshSlotManagerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<NavMeshSlotManager> Manager;
    [SerializeReference] public BlackboardVariable<NavMeshSlot> Navmeshslot;

    public override bool IsTrue()
    {
        if (Navmeshslot.Value == null) return false; 
        if (Navmeshslot.Value.SlotManager == null) return false;

        Manager.Value = Navmeshslot.Value.SlotManager;

        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
