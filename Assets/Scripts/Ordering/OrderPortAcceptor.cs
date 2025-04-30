using Unity.Netcode;
using UnityEngine;

public class OrderPortAcceptor : NetworkBehaviour
{
    OrderPort orderPort;

    public void Init(OrderPort orderPort)
    {
        this.orderPort = orderPort;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (!other.TryGetComponent(out NetworkObject networkObject)) return;

        if (other.TryGetComponent(out OrderContainerBox orderContainerbox))
        {
            Contents contents = orderContainerbox.Contents;

            if (contents != null && orderPort != null)
            {
                orderPort.ProcessOrderBox(contents);
            }
        }

        if (other.transform.root.gameObject.TryGetComponent(out PlayerCharacterController characterController)) { return; }

        networkObject.Despawn();
    }
}
