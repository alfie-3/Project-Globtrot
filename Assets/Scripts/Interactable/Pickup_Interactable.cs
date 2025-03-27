using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup_Interactable : NetworkBehaviour, IInteractable, IOnDrop
{
    public NetworkVariable<bool> PickedUp = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [field: SerializeField] public PlayerObjectSocketManager.ObjectSocket HoldingSocket { get; private set; }

    public Action OnPickedUp = delegate { };
    public Action OnDropped = delegate { };

    public virtual void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            if (holdingManager.HeldObj != null) return;
        }

        Pickup(interactionManager);
        Pickup_RPC();
    }

    protected void Pickup(PlayerInteractionManager interactionManager)
    {
        if (PickedUp.Value == true) return;

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.HoldItem(this);
        }
    }



    public void OnDrop(PlayerHoldingManager holdingManager)
    {
        if (PickedUp.Value == false) return;

        Drop_RPC();
        OnDropped.Invoke();
    }


    [Rpc(SendTo.Everyone)]
    private void Pickup_RPC()
    {
        if (IsServer)
            PickedUp.Value = true;

        OnPickedUp.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    private void Drop_RPC()
    {
        if (IsServer)
            PickedUp.Value = false;

        OnDropped.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void RequestRemove_RPC()
    {
        PickedUp.Value = true;
        NetworkObject.Despawn();

    }
}
