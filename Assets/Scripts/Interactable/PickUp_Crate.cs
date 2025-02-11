using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PickUp_Crate : Pickup_Interactable {
    [SerializeField] public ItemBase item;
    NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public virtual void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");

        Pickup(interactionManager);
        Pickup_RPC();
    }

    protected void Pickup(PlayerInteractionManager interactionManager)
    {
        if (pickedUp.Value == true) return;

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.HoldItem(item,this);
            
        }
    }


    
    public void OnDrop(PlayerHoldingManager holdingManager) 
    {
        if (pickedUp.Value == false) return;
        holdingManager.ClearItem();
        Drop_RPC();
    }


    [Rpc(SendTo.Server)]
    private void Pickup_RPC() {
        pickedUp.Value = true;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        NetworkRigidbody nBody = GetComponent<NetworkRigidbody>();
        nBody.UseRigidBodyForMotion = false;

        transform.localScale = Vector3.one * 0.25f;
    }

    [Rpc(SendTo.Server)]
    private void Drop_RPC() {
        pickedUp.Value = false;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;

        NetworkRigidbody nBody = GetComponent<NetworkRigidbody>();
        nBody.UseRigidBodyForMotion = true;

        transform.localScale = Vector3.one;
    }


    [Rpc(SendTo.Server)]
    public void RequestRemove_RPC()
    {
        pickedUp.Value = true;
        NetworkObject.Despawn();
        
    }

    public void OnUnview()
    {
        Debug.Log("Unview");
    }

    public void OnView()
    {
        Debug.Log("Viewed");
    }
}
