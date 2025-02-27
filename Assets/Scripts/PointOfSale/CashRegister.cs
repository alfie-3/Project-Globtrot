using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Netcode;
using UnityEngine;

public class CashRegister : NetworkBehaviour, IInteractable
{
    public static List<CashRegister> CashRegisters = new();
    [field: SerializeField] public NavMeshSlotManager Queue { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        CashRegisters = new List<CashRegister>();
    }

    private void OnEnable()
    {
        CashRegisters.Add(this);
    }

    private void OnDisable()
    {
        CashRegisters.Remove(this);
    }

    public static bool TryGetRandomCashRegister(out CashRegister cashRegister)
    {
        cashRegister = null;

        if (CashRegisters.Count == 0) return false;

        cashRegister = CashRegisters[Random.Range(0, CashRegisters.Count)];

        return true;
    }

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        ProcessCustomer_Rpc();
    }

    [Rpc(SendTo.Server)]
    public void ProcessCustomer_Rpc()
    {
        if (Queue.NavMeshSlots[0].OccupyingGameObject == null) return;

        if (!Queue.NavMeshSlots[0].OccupyingGameObject.TryGetComponent(out BasicCustomer customer)) return;

        if (customer.TryGetComponent(out BehaviorGraphAgent behaviourGraph))
        {
            if (!behaviourGraph.BlackboardReference.GetVariable("CustomerProcessed", out BlackboardVariable<CustomerProcessed> customerProcessed)) return;

            customerProcessed.Value.SendEventMessage();
            customer.PurchaseItems();
        }
    }

    public void OnView()
    {
    }

    public void OnUnview()
    {
    }
}
