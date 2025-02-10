using Unity.Netcode;
using UnityEngine;

public class CharacterColliderPush : NetworkBehaviour
{
    [SerializeField] float CharacterRepelForce = 10000f;
    [SerializeField] float pushPower = 0.2f;

    CharacterMovement characterMovement;
    LayerMask mask = ~0;

    private void Awake()
    {
        characterMovement = GetComponentInParent<CharacterMovement>();
    }

    private void Update()
    {
        PerformCollisions();
    }

    private void PerformCollisions()
    {
        Vector3 start = new(transform.position.x, transform.position.y + characterMovement.Controller.height/2, transform.position.z);
        Vector3 end = new(transform.position.x, transform.position.y - characterMovement.Controller.height/2, transform.position.z);

        Debug.DrawLine(start, end, Color.green);
        Collider[] colliders = Physics.OverlapCapsule(start, end, 0.5f, mask, QueryTriggerInteraction.Collide);

        Debug.Log(colliders.Length);

        foreach (Collider collider in colliders)
        {
            Vector3 direction = collider.gameObject.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if ((LayerMask.GetMask("Player") & (1 << collider.gameObject.layer)) != 0)
            {
                Debug.Log(collider.gameObject.name);
                if (!IsLocalPlayer) return;
                if (transform.parent == collider.transform) return;
                if (transform == collider.transform) return;

                characterMovement.Push(-direction * CharacterRepelForce);

                return;
            }

            Rigidbody rb = collider.attachedRigidbody;
            if (rb == null) return;

            rb.AddForceAtPosition(direction * pushPower, transform.position, ForceMode.Impulse);
        }

    }
}

