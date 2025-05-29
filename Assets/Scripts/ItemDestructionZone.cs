using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDestructionZone : NetworkBehaviour
{
    [SerializeField] ParticleSystem destructionEffect;
    private void OnTriggerEnter(Collider other)
    {
        DestroyItem(other.gameObject);
    }

    protected virtual void DestroyItem(GameObject other)
    {
        if (!IsServer) { return; }

        if (other.TryGetComponent(out Pickup_Interactable item))
        {
            if (item.NetworkObject != null)
            {
                if (!item.NetworkObject.IsSpawned) return;

                item.NetworkObject.Despawn();
                PlayParticle_Rpc();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void PlayParticle_Rpc()
    {
        if (destructionEffect != null)
        {
            Vector3 effectPos = new(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z);
            Quaternion effectRot = Quaternion.Euler(-90f, 0, 0);
            Instantiate(destructionEffect, effectPos, effectRot);
        }
    }
}
