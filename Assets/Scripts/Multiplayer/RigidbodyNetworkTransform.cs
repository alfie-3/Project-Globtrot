using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody))]
public class RigidbodyNetworkTransform : NetworkTransform
{
    //References
    public Rigidbody Rigidbody { get; private set; }
    public NetworkRigidbody NetworkRigidbody { get; private set; }

    //Sleping
    public bool IsSleeping = false;
    public Action<bool> OnSleepingChanged = delegate { };

    //Collision detection for detecting sleeping state
    public bool IsColliding;

    //For rigidbody teleportation
    public bool AwaitingTeleportUpdate { get; private set; } = false;
    public Action OnTeleportUpdate = delegate { };

    //Socketing
    public bool IsSocketed = false;

    public bool rigidbodyDisabled;

    protected override void Awake()
    {
        base.Awake();

        Rigidbody = GetComponent<Rigidbody>();
        NetworkRigidbody = GetComponent<NetworkRigidbody>();

        OnSleepingChanged += UpdateObjectPhysics;
    }
    public void Update()
    {
        if (IsSleeping == true || rigidbodyDisabled) return;

        CheckPhysicsState();
    }

    public void OnCollisionStay(Collision collision) { IsColliding = true; }

    public void FixedUpdate() { IsColliding = false; }

    protected override void OnNetworkTransformStateUpdated(ref NetworkTransformState oldState, ref NetworkTransformState newState)
    {
        base.OnNetworkTransformStateUpdated(ref oldState, ref newState);

        if (AwaitingTeleportUpdate == true && newState.IsTeleportingNextFrame)
        {
            AwaitingTeleportUpdate = false;
            OnTeleportUpdate.Invoke();
            Interpolate = true;
        }
    }

    public void WaitForNextTeleport()
    {
        AwaitingTeleportUpdate = true;
        Interpolate = false;
    }

    public void SetSleeping(bool state)
    {
        IsSleeping = state;
        OnSleepingChanged.Invoke(state);    
    }

    public void WakeUp()
    {
        if (!IsSleeping || rigidbodyDisabled) return;

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

    public void SetRigidbodyEnabled(bool state)
    {
        rigidbodyDisabled = state;

        if (IsServer)

        Rigidbody.isKinematic = !state;
        NetworkRigidbody.UseRigidBodyForMotion = state;
    }

    public void CheckPhysicsState()
    {
        if (rigidbodyDisabled) return;
        if (NetworkRigidbody.GetLinearVelocity().magnitude > 0.001f) return;
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

    [Rpc(SendTo.Server)]
    public void SetLinearVelocity_Rpc(Vector3 velocity)
    {
        Rigidbody.linearVelocity = velocity;
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

#if UNITY_EDITOR
[CustomEditor(typeof(RigidbodyNetworkTransform))]
public class RBNetworkTransform : Editor
{
    public override void OnInspectorGUI()
    {
        RigidbodyNetworkTransform rBNetwork = (RigidbodyNetworkTransform)target;

        DrawDefaultInspector();
    }
}
#endif