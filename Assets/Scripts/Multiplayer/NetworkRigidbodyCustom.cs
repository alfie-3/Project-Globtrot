using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkRigidbodyCustom : NetworkRigidbody
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
            GetComponent<Rigidbody>().isKinematic = true;
    }

    [Rpc(SendTo.Server)]
    public void ApplyForce_Rpc(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }
}
