using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup_Interactable : NetworkBehaviour, IInteractable
{
    [SerializeField] ItemBase item;
    NetworkVariable<bool> pickedUp = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public void OnInteract(PlayerInteractionManager interactionManager)
    {
        Debug.Log("Interacted");

        Pickup(interactionManager);
    }

    public void Pickup(PlayerInteractionManager interactionManager)
    {
        if (pickedUp.Value == true) return;

        if (interactionManager.TryGetComponent(out PlayerHoldingManager holdingManager))
        {
            holdingManager.HoldItem(item);

            RequestRemove_RPC();
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestRemove_RPC()
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
