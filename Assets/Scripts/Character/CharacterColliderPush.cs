using Unity.Netcode;
using UnityEngine;

public class CharacterColliderPush : NetworkBehaviour
{
    [SerializeField] float radius = 0.21f;
    [Space]
    [SerializeField] float CharacterRepelForce = 10000f;
    [SerializeField] float pushPower = 0.2f;

    CharacterMovement characterMovement;
    LayerMask mask = ~0;

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

        Collider[] colliders = Physics.OverlapCapsule(start, end, radius, mask, QueryTriggerInteraction.Collide);

        foreach (Collider collider in colliders)
        {
            if (collider.transform == transform) continue;

            if (collider.TryGetComponent(out CharacterColliderPush _))
            {
                Debug.Log("Push");
                PerformPlayerCollision(collider);
            }

            Vector3 direction = collider.gameObject.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            Rigidbody rb = collider.attachedRigidbody;
            if (rb == null) continue;

            rb.AddForceAtPosition(direction * pushPower, transform.position, ForceMode.Impulse);
        }
    }

    private void PerformPlayerCollision(Collider collider)
    {
        Vector3 direction = collider.gameObject.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        if (!IsOwner) return;

        characterMovement.Push(-direction * CharacterRepelForce);

        return;
    }
}

