using UnityEngine;

public class CharacterControllerPush : MonoBehaviour
{
    [SerializeField] float pushPower = 0.2f;
    //[SerializeField] float weight = 6f;

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

        Vector3 direction = collision.gameObject.transform.position - transform.forward;
        direction.y = 0;
        direction.Normalize();

        rb.AddForceAtPosition(direction * pushPower, collision.contacts[0].point, ForceMode.Impulse);
    }
}
