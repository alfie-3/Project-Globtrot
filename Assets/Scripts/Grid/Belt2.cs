using System;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class Belt2 : MonoBehaviour
{
    
    [field: SerializeField] public float rotation { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //[field: SerializeField] public static float  { get; private set; }

    public Action<Belt2> OnDestroyAction = delegate { };
    Rigidbody rigidbody;
    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
        BeltManager.AddMe(GetComponent<Belt2>(), ref OnDestroyAction);
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
        Vector3 pos = rigidbody.position;
        rigidbody.position -= (Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward) * speed * Time.fixedDeltaTime;
        rigidbody.MovePosition(pos);
    }



    

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position,transform.position + (Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward));
    }
}
