using Unity.Netcode;
using UnityEngine;

public class OrderBoxInput : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent(out StockItem stockItem))
        {
            if (stockItem.Item == null) return;

            if (!transform.parent.TryGetComponent(out IContents contents)) return;

            if (!contents.Contents.TryAddItem(stockItem.Item.ItemID, 1)) return;

            if (other.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.Despawn();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
