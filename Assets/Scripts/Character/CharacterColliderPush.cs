using UnityEngine;

public class CharacterColliderPush : MonoBehaviour
{
    [SerializeField] float pushPower = 0.2f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;


        Vector3 direction = other.gameObject.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        rb.AddForceAtPosition(direction * pushPower, transform.position, ForceMode.Impulse);
    }
}

