using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDestructionZone : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }

        if (other.TryGetComponent(out Pickup_Interactable item))
        {
            if (item.NetworkObject != null)
            {
                if (!NetworkObject.IsSpawned) return;

                item.NetworkObject.Despawn();
            }
        }
    }
}
