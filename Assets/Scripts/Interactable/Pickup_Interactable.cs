using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_Interactable : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemBase item;
    NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public virtual void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");

        Pickup(interactionManager);
        ChangeParent_RPC(true, interactionManager.GetComponent<NetworkObject>());
    }

    public void Pickup(PlayerInteractionManager interactionManager)
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
        RemoveParent_RPC();
    }


    [Rpc(SendTo.Server)]
    private void ChangeParent_RPC(bool state,NetworkObjectReference awd) {
        pickedUp.Value = state;

        if (awd.TryGet(out NetworkObject ham)) {
            Rigidbody body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            NetworkRigidbody nBody = GetComponent<NetworkRigidbody>();
            nBody.UseRigidBodyForMotion = false;

            transform.localScale = Vector3.one * 0.25f;
        }

    }

    [Rpc(SendTo.Server)]
    private void RemoveParent_RPC() {
        pickedUp.Value = false;

        //GetComponent<NetworkObject>().TryRemoveParent();
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
