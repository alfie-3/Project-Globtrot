using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_Interactable : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemBase item;

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            if (!IsOwner)
                NetworkObject.ChangeOwnership(interactionManager.OwnerClientId);

            holdingManager.HoldItem(item);
            NetworkObject.Despawn();
        }
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
