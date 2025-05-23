using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class InfiniteItemGrabber : NetworkBehaviour, IInteractable
{
    [SerializeField] NetworkObject itemPrefab;
    [Space]
    [SerializeField] int itemLimit = 15;
    int spawnedItems;

    [SerializeField] NetworkAnimator animator;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        if (interactionManager.GetComponent<PlayerHoldingManager>().HoldingItem) return;


        GiveItem_Rpc(interactionManager.gameObject, interactionManager.OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    private void GiveItem_Rpc(NetworkObjectReference holderReference, ulong clientID)
    {
        if (!holderReference.TryGet(out NetworkObject obj))
            return;

        if (spawnedItems >= itemLimit) return;

        if (animator != null)
        {
            animator.SetTrigger("Close");
        }

        NetworkObject instance = Instantiate(itemPrefab, obj.transform.position, transform.rotation).GetComponent<NetworkObject>();
        instance.Spawn();

        if (instance.TryGetComponent(out RigidbodyNetworkTransform rbNT))
        {
            spawnedItems++;

            if (animator)
                rbNT.OnDespawned += () => { spawnedItems--; animator.Animator.SetBool("ReOpen", spawnedItems < itemLimit); };
        }

        if (animator)
            animator.Animator.SetBool("ReOpen", spawnedItems < itemLimit);

        GivItem_Rpc(instance, holderReference, RpcTarget.Single(clientID, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void GivItem_Rpc(NetworkObjectReference newInstanceRef, NetworkObjectReference holderReference, RpcParams rpcParams)
    {
        if (!holderReference.TryGet(out NetworkObject holder))
            return;

        if (!newInstanceRef.TryGet(out NetworkObject newInstance))
        {
            Debug.Log("NewInstanceNull");
            return;
        }

        newInstance.GetComponent<Pickup_Interactable>().OnInteract(holder.GetComponent<PlayerInteractionManager>());
    }
}
