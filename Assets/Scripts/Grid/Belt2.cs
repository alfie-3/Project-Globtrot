using System;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class Belt2 : MonoBehaviour
{
    public float speed = 1.0f;
    [field: SerializeField] public float rotation { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //[field: SerializeField] public static float  { get; private set; }

    public Action<Rigidbody> OnDestroyAction = delegate { };
    //Rigidbody rigidbody;
    private void Start() {
        BeltManager.AddMe(GetComponent<Rigidbody>(),OnDestroyAction);
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.transform.parent == null) return;

        if (!collision.collider.transform.parent.TryGetComponent(out CharacterMovement characterMovement)) return;
        characterMovement.Push((Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward).normalized * BeltManager.Instance.PlayerForce);
    }
    private void FixedUpdate() {
        //Vector3 pos = rigidbody.position;
        //rigidbody.position -= transform.forward * speed * Time.fixedDeltaTime;
        //rigidbody.MovePosition(pos);
    }

    private void OnDestroy() {
        OnDestroyAction.Invoke(GetComponent<Rigidbody>());
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position + (Quaternion.AngleAxis(rotation,Vector3.up) * transform.forward));
    }
}
