using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class HoldingItemSocket : NetworkBehaviour
{
    NetworkObject boundObject;

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
            boundObject = bindingObject;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClearObjectBindingServer_Rpc(Vector3 position, Quaternion rotation)
    {
        if (boundObject == null) return;
        if (!boundObject.TryGetComponent(out NetworkTransform networkTransform)) return;

        if(IsServer)
         networkTransform.Teleport(position, rotation, boundObject.transform.localScale);

        boundObject = null;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ClearObjectBindingClient_Rpc()
    {
        if (boundObject == null) return;
        if (!boundObject.TryGetComponent(out NetworkTransform networkTransform)) return;

        boundObject = null;
    }
}
