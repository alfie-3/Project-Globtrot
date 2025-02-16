using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody))]
public class RigidbodyNetworkTransform : NetworkTransform
{
    public bool IsSleeping = false;
    public Action<bool> OnSleepingChanged = delegate { };

    public Rigidbody Rigidbody { get; private set; }
    public NetworkRigidbody NetworkRigidbody { get; private set; }

    public bool IsColliding;

    public bool AwaitingNextUpdate { get; private set; } = false;

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

    protected override void OnNetworkTransformStateUpdated(ref NetworkTransformState oldState, ref NetworkTransformState newState)
    {
        base.OnNetworkTransformStateUpdated(ref oldState, ref newState);

        if (AwaitingNextUpdate == true && oldState.HasPositionChange)
        {
            AwaitingNextUpdate = false;

            Interpolate = true;

            SyncPositionX = true;
            SyncPositionY = true;
            SyncPositionZ = true;

            SyncRotAngleX = true;
            SyncRotAngleY = true;
            SyncRotAngleZ = true;
        }
    }

    public void AwaitNextTransformUpdate()
    {
        AwaitingNextUpdate = true;

        SyncPositionX = false;
        SyncPositionY = false;
        SyncPositionZ = false;

        SyncRotAngleX = false;
        SyncRotAngleY = false;
        SyncRotAngleZ = false;

        Interpolate = false;
    }

    public void SetSleeping(bool state)
    {
        IsSleeping = state;
        OnSleepingChanged.Invoke(state);    
    }

    public void WakeUp()
    {
        if (!IsSleeping) return;

        SetSleeping(false);

        WakeUpNearbyObjects();
    }

    public void WakeUpNearbyObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<Collider>().bounds.max.magnitude);

        foreach(Collider collider in colliders)
        {
            if (collider.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
            {
                rbNWT.WakeUp();
            }
        }
    }

    public void CheckPhysicsState()
    {
        if (Rigidbody.linearVelocity.magnitude > 0.001) return;
        if (!IsColliding) return;

        SetSleeping(true);
    }

    [Rpc(SendTo.Everyone)]
    public void AddForce_Rpc(Vector3 force, ForceMode forceMode)
    {
        SetSleeping(false);

        if (IsServer)
            Rigidbody.AddForce(force, forceMode);
    }

    [Rpc(SendTo.Everyone)]
    public void AddForceAtPoint_Rpc(Vector3 force, Vector3 point, ForceMode forceMode)
    {
        SetSleeping(false);

        if (IsServer)
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
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Physics");
            NetworkRigidbody.UseRigidBodyForMotion = true;
        }
    }
}
