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
    private void Awake() {
        BeltManager.AddMe(GetComponent<Rigidbody>(),OnDestroyAction);
    }
    void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("gam");
    }

    private void OnCollisionStay(Collision collision) {
        Debug.Log("Colisino");
        if(collision.collider.gameObject.layer != BeltManager.Instance.PlayerLayer) return;

        Debug.Log("Layer");
        if (!collision.collider.transform.parent.TryGetComponent<CharacterMovement>(out CharacterMovement characterMovement)) return;
        Debug.Log("Player");
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
