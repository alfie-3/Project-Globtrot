using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PhysicsObjectHealth))]
public class GasCanister : NetworkBehaviour
{
    [SerializeField] TMP_Text text;
    [field: SerializeField] public bool OverPressurised { get; private set; }
    [SerializeField] ParticleSystem gasParticles;
    [SerializeField] GameObject explosionEffect;
    [Space]
    [SerializeField] float explosionRadius = 5;
    [SerializeField] float explosionPower = 100;

    private void Awake()
    {
        GetComponent<StockItem>().OnItemChanged += UpdateGasType;
    }

    public void UpdateGasType(Stock_Item item)
    {
        string name = item.ItemName;
        name = name.Replace(" Canister", "");
        text.text = name;
    }

    [Rpc(SendTo.Everyone)]
    public void OverPressurize_Rpc()
    {
        OverPressurised = true;
        GetComponent<PhysicsObjectHealth>().ShouldTakeDamage = true;
        gasParticles.Play();
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 4f);
        Collider[] rigidbodyColliders = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Default", "Physics", "Player", "PlayerCollider"));

        foreach (Collider col in rigidbodyColliders)
        {
            if (col.TryGetComponent(out PlayerCharacterController playerCharacter))
            {
                playerCharacter.SetRagdoll(true);
                playerCharacter.Knockout(4);

                Vector3 explosionDirection = Vector3.Normalize(playerCharacter.transform.position - transform.position);
                explosionDirection += Vector3.up;

                playerCharacter.CharacterMovement.Push(explosionDirection * explosionPower / 8);
            }

            if (col.attachedRigidbody != null)
            {
                if (col.TryGetComponent(out RigidbodyNetworkTransform rbNT))
                {
                    rbNT.WakeUp();

                    if (IsServer)
                    {
                        col.attachedRigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius);
                    }

                    continue;
                }

                col.attachedRigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius);

            }
        }
    }
}
