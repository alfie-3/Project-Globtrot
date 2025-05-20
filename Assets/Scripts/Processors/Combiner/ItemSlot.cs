using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemSlot : NetworkBehaviour, IUseItem
{
    public NetworkVariable<FixedString32Bytes> itemId = new NetworkVariable<FixedString32Bytes>();

    [field: SerializeField] public GameObject Item {get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    public abstract bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item);

    public void OnItemUsed(PlayerHoldingManager manager, Stock_Item shopProduct_Item)
    {
        if (!string.IsNullOrEmpty(itemId.Value.ToString())) return;
        if (CanUseItem(manager, shopProduct_Item))
        {
            NetworkObject nwObj = manager.HeldObj;
            manager.ClearHeldItem(transform.position, transform.rotation.eulerAngles);
            ConnectItem_Rpc(nwObj);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ConnectItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        nwObject.GetComponent<Pickup_Interactable>().OnPickedUp += ItemRemoved;
        Item = nwObject.gameObject;

        nwObject.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(false);

        

        if (!IsServer) return;
        itemId.Value = Item.GetComponent<StockItem>().Item.ItemID;

        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }

    public void ClearItem()
    {
        DeleteItem_Rpc(Item.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Server)]
    private void DeleteItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        Item = null;
        itemId.Value = string.Empty;

        nwObject.Despawn();
    }

    public void ItemRemoved()
    {
        Pickup_Interactable pickup_Interactable = Item.GetComponent<Pickup_Interactable>();
        pickup_Interactable.OnPickedUp -= ItemRemoved;

        Item.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(true);
        Item = null;

        if(IsServer)
            itemId.Value = string.Empty;
    }

    public void SetGasCannisterType(Stock_Item item)
    {
        Item.GetComponent<StockItem>().SetItem(item);
    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        if(!string.IsNullOrEmpty(itemId.Value.ToString()))
            return new(false);
        if (CanUseItem(holdingManager, item))
        {
            return new(true, "Insert");
        }

        return new(false);
    }
    public void OnUnview()
    {

    }

    private void OnDestroy()
    {
        if (Item != null)
            ItemRemoved();
    }
}
