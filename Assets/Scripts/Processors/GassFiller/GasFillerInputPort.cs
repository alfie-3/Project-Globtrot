using Unity.Netcode;
using UnityEngine;

public class GasFillerInputPort : NetworkBehaviour, IUseItem
{
    [SerializeField] ShopProduct_Item referenceItem;
    NetworkVariable<bool> filled = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            filled.Value = false;
    }

    public void OnItemUsed(PlayerHoldingManager manager, ShopProduct_Item shopProduct_Item)
    {
        if (shopProduct_Item == referenceItem)
        {
            if (filled.Value == true) return;

            NetworkObject heldobject = manager.HeldObj;
            manager.DisconnectHeldObject_Rpc();

            ConnectItem_Rpc(heldobject);
        }
    }

    [Rpc(SendTo.Server)]
    public void ConnectItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        filled.Value = true;

        nwObject.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(false);
        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }
}
