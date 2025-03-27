using System;
using UnityEngine;
[RequireComponent (typeof(Rigidbody))]
public class BeltSingle : Belt
{
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float playerForce { get; private set; }
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Jiggle(speed);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.parent == null) return;

        if (!collision.collider.transform.parent.TryGetComponent(out CharacterMovement characterMovement)) return;
        characterMovement.Push((Quaternion.AngleAxis(rotation, Vector3.up) * transform.forward).normalized * playerForce);
    }
}
