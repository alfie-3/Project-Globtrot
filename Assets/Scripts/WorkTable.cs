using UnityEngine;

public class WorkTable : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out OrderContainerBox box))
        {
            box.SetOpen(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out OrderContainerBox box))
        {
            if (collision.attachedRigidbody.isKinematic) return;
            box.SetOpen(false);
        }
    }
}
