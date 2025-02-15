using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class RigidbodyNetworkTransform : NetworkTransform
{
    public bool IsSleeping = false;
    public Action<bool> OnSleepingChanged = delegate { };

    public Rigidbody Rigidbody { get; private set; }
    public NetworkRigidbody NetworkRigidbody { get; private set; }

    public bool IsColliding;

    protected override void Awake()
    {
        base.Awake();

        Rigidbody = GetComponent<Rigidbody>();
        NetworkRigidbody = GetComponent<NetworkRigidbody>();

        OnSleepingChanged += UpdateObjectPhysics;
    }

    public void Update()
    {
        if (IsSleeping == true) return;

        CheckPhysicsState();
    }

    public void OnCollisionStay(Collision collision) { IsColliding = true; }
    public void FixedUpdate() { IsColliding = false; }

    public void SetSleeping(bool state)
    {
        IsSleeping = state;
        OnSleepingChanged.Invoke(state);    
    }

    public void WakeUp()
    {
        SetSleeping(false);
    }

    public void CheckPhysicsState()
    {
        if (IsClient)
            Debug.Log($"{IsColliding} : {Rigidbody.linearVelocity.magnitude}");

        if (Rigidbody.linearVelocity.magnitude > 0.001) return;
        if (!IsColliding) return;

        SetSleeping(true);
    }

    [Rpc(SendTo.Everyone)]
    public void AddForce_Rpc(Vector3 force, ForceMode forceMode)
    {
        SetSleeping(false);
        Rigidbody.AddForce(force, forceMode);
    }

    [Rpc(SendTo.Everyone)]
    public void AddForceAtPoint_Rpc(Vector3 force, Vector3 point, ForceMode forceMode)
    {
        SetSleeping(false);
        Rigidbody.AddForceAtPosition(force, point, forceMode);
    }

    public void UpdateObjectPhysics(bool current)
    {
        NetworkRigidbody.UseRigidBodyForMotion = current;

        if (IsServer)
            Rigidbody.isKinematic = current;

        if (current == true)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            NetworkRigidbody.ApplyCurrentTransform();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Physics");
            NetworkRigidbody.UseRigidBodyForMotion = true;
        }
    }
}
