using System;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class Belt : MonoBehaviour
{
    [field: SerializeField] public float rotation { get; private set; }

    public Action<Belt> OnDestroyAction = delegate { };
    protected Rigidbody rb;
    private void Start() {
        rb = GetComponent<Rigidbody>();
        BeltManager.AddMe(GetComponent<Belt>(), ref OnDestroyAction);
    }

    private void OnDestroy() {
        OnDestroyAction.Invoke(this);
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.transform.parent == null) return;

        if (!collision.collider.transform.parent.TryGetComponent(out CharacterMovement characterMovement)) return;
        characterMovement.Push((Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward).normalized * BeltManager.Instance.PlayerForce);
    }


    public void Jiggle(float speed)
    {
        Vector3 pos = rb.position;
        rb.position -= (Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward) * speed * Time.fixedDeltaTime;
        rb.MovePosition(pos);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position + (Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward));
    }
}
