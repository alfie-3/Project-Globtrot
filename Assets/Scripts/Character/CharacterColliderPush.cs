using Unity.Netcode;
using UnityEngine;

public class CharacterColliderPush : NetworkBehaviour
{
    [SerializeField] float radius = 0.21f;
    [Space]
    [SerializeField] float CharacterRepelForce = 10000f;
    [SerializeField] float pushPower = 0.2f;

    CharacterMovement characterMovement;
    [SerializeField] LayerMask mask;

    Collider[] allocatedColliders = new Collider[20];

    private void Awake()
    {
        characterMovement = GetComponentInParent<CharacterMovement>();
    }

    private void FixedUpdate()
    {
        PerformCollisions();
    }

    private void PerformCollisions()
    {
        Vector3 start = new(transform.position.x, transform.position.y + characterMovement.Controller.height/2, transform.position.z);
        Vector3 end = new(transform.position.x, transform.position.y - characterMovement.Controller.height/2, transform.position.z);

        int collidersCount = Physics.OverlapCapsuleNonAlloc(start, end, radius, allocatedColliders, mask, QueryTriggerInteraction.Collide);

        Collider collider;

        for (int i = 0; i < collidersCount; i++)
        {
            collider = allocatedColliders[i];

            if (collider.transform == transform) continue;

            Vector3 direction = collider.gameObject.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (collider.TryGetComponent(out CharacterColliderPush _))
            {
                PerformPlayerCollision(collider, direction);
            }

            if (collider.attachedRigidbody != null)
            {
                PerformPhysicsCollisions(collider, direction);
            }
        }
    }

    private void PerformPlayerCollision(Collider collider, Vector3 direction)
    {
        if (!IsOwner) return;

        characterMovement.Push(-direction * CharacterRepelForce);

        return;
    }

    private void PerformPhysicsCollisions(Collider collider, Vector3 direction)
    {
        
        if (collider.TryGetComponent(out RigidbodyNetworkTransform rigidbodyNetworkTransform))
        {
            rigidbodyNetworkTransform.AddForceAtPoint_Rpc(direction * pushPower, transform.position, ForceMode.Force);
        }
        else
            collider.attachedRigidbody.AddForceAtPosition(direction * pushPower, transform.position, ForceMode.Force);
    }
}

