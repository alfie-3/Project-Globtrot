using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class HoldingItemSocket : NetworkBehaviour
{
    [SerializeField] NetworkObject boundObject;

    // Update is called once per frame
    void Update()
    {
        if (boundObject == null) return;

        boundObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    [Rpc(SendTo.Everyone)]
    public void BindObject_Rpc(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject bindingObject))
        {
            if (bindingObject.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
            {
                rbNWT.WakeUpNearbyObjects();
                rbNWT.IsSocketed = true;
            }

            boundObject = bindingObject;
        }

        ToggleBoundObjectCollisions(false);
    }

    [Rpc(SendTo.Everyone)]
    public void ClearObjectBinding_Rpc(Vector3 position, Quaternion rotation)
    {
        if (boundObject == null) return;
        if (!boundObject.TryGetComponent(out NetworkTransform networkTransform)) return;

        if (boundObject.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
        {
            ToggleBoundObjectCollisions(true);
            rbNWT.WakeUp();

            rbNWT.transform.SetPositionAndRotation(position, rotation);

            if (!IsLocalPlayer || IsServer)
            {
                if (IsServer)
                    networkTransform.Teleport(position, rotation, boundObject.transform.localScale);

                rbNWT.IsSocketed = false;
                rbNWT.NetworkRigidbody.ApplyCurrentTransform();
                boundObject = null;
            }
            else 
            {
                //Delays dropping item until the server teleports the item to reduce flickering effect while the object syncs

                rbNWT.IsSocketed = false;

                rbNWT.WaitForNextTeleport();

                Action clearAction = () =>
                {
                    ClearBoundObject();
                };

                rbNWT.OnTeleportUpdate += () => {
                    clearAction.Invoke();
                    rbNWT.OnTeleportUpdate -= clearAction;
                };
            }
        }
    }

    public void ClearBoundObject()
    {
        boundObject = null;
    }

    public void ToggleBoundObjectCollisions(bool toggle)
    {
        if (boundObject == null) return;

        foreach (Collider collider in boundObject.GetComponents<Collider>())
        {
            collider.enabled = toggle;
        }

        foreach (Collider collider in boundObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = toggle;
        }
    }

    public void ToggleSyncing(bool toggle)
    {
        if (!boundObject.TryGetComponent(out NetworkTransform networkTransform)) return;

        networkTransform.SyncPositionX = toggle;
        networkTransform.SyncPositionY = toggle;
        networkTransform.SyncPositionZ = toggle;
    }
}

