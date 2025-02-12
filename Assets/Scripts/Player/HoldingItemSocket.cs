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

            boundObject.GetComponent<NetworkTransform>().enabled = false;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ClearObjectBinding_Rpc()
    {
        if (boundObject == null) return;

        boundObject.GetComponent<NetworkTransform>().enabled = true;
        boundObject = null;
    }
}
