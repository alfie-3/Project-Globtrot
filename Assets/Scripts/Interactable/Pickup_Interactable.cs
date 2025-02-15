using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Pickup_Interactable : NetworkBehaviour, IInteractable, IOnDrop
{
    NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

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
        if (pickedUp.Value == true) return;

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.HoldItem(this);
        }
    }



    public void OnDrop(PlayerHoldingManager holdingManager)
    {
        if (pickedUp.Value == false) return;
        holdingManager.ClearItem();
        Drop_RPC();
    }


    [Rpc(SendTo.Everyone)]
    private void Pickup_RPC()
    {
        if (IsServer)
        {
            pickedUp.Value = true;
            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            NetworkRigidbody nBody = GetComponent<NetworkRigidbody>();
            nBody.UseRigidBodyForMotion = false;
        }

        ToggleCollisions(false);
    }

    [Rpc(SendTo.Everyone)]
    private void Drop_RPC()
    {
        if (IsServer)
        {
            pickedUp.Value = false;
        }

        if (TryGetComponent(out RigidbodyNetworkTransform rbNWT))
        {
            rbNWT.WakeUp();
        }

        ToggleCollisions(true);

    }

    public void ToggleCollisions(bool toggle)
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = toggle;
        }

        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = toggle;
        }
    }


    [Rpc(SendTo.Server)]
    public void RequestRemove_RPC()
    {
        pickedUp.Value = true;
        NetworkObject.Despawn();

    }

    public void OnUnview()
    {
    }

    public void OnView()
    {
    }
}
