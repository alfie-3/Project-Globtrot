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

            ToggleHeldObjectCollisions(false);
        }
    }

    [Rpc(SendTo.Everyone, DeferLocal = true)]
    public void ClearObjectBindingServer_Rpc(Vector3 position, Quaternion rotation)
    {
        if (boundObject == null) return;

        boundObject.transform.SetPositionAndRotation(position, rotation);

        ToggleHeldObjectCollisions(true);
        boundObject = null;

    }


    public void ToggleHeldObjectCollisions(bool toggle)
    {
        foreach (Collider collider in boundObject.GetComponents<Collider>())
        {
            collider.enabled = toggle;
        }

        foreach (Collider collider in boundObject.GetComponentsInChildren<Collider>())
        {
            collider.enabled = toggle;
        }
    }
}
