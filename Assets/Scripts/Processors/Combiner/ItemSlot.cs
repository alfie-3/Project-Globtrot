using System;
using Unity.Netcode;
using UnityEngine;

public abstract class ItemSlot : NetworkBehaviour, IUseItem
{
    

    public Action<Stock_Item> OnItemAdded = delegate { };
    public Action OnItemRemoved = delegate { };

    [field: SerializeField] public GameObject Item {get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

    }

    public abstract bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item);

    public void OnItemUsed(PlayerHoldingManager manager, Stock_Item shopProduct_Item)
    {
        if (Item != null) return;
        if (CanUseItem(manager, shopProduct_Item))
        {
            OnItemAdded.Invoke(shopProduct_Item);

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

        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }

    public void ItemRemoved()
    {
        Pickup_Interactable pickup_Interactable = Item.GetComponent<Pickup_Interactable>();
        pickup_Interactable.OnPickedUp -= ItemRemoved;
        OnItemRemoved.Invoke();

        Item.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(true);
        Item = null;

        
    }

    public void SetGasCannisterType(Stock_Item item)
    {
        Item.GetComponent<StockItem>().SetItem(item);
    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {

        if (CanUseItem(holdingManager, item))
        {
            return new(true, "Insert");
        }

        return new(false);
    }
    public void OnUnview()
    {

    }
}
