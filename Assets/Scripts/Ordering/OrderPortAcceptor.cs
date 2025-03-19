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
        IContents boxContents = null;

        if (other.TryGetComponent(out OrderContainerBox orderContainerbox)) { boxContents = orderContainerbox; }    

        if (boxContents == null) return;

        if (orderPort == null) return;

        orderPort.ProcessOrderBox(boxContents.Contents);

        if (other.TryGetComponent(out NetworkObject networkObject))
        {
            networkObject.Despawn();
        }
    }
}
