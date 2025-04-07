using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class StockItem : NetworkBehaviour
{
    [field: SerializeField] public Stock_Item Item { get; private set; }

    public Action<Stock_Item> OnItemChanged = delegate { };

    public void SetItem(Stock_Item item)
    {
        if (!IsServer) return;

        Item = item;
        OnItemChanged(item);

        SetItem_Rpc(item.ItemID);
    }

    [Rpc(SendTo.Everyone)]
    public void SetItem_Rpc(string itemID)
    {
        if (IsServer) return;

        Item = ItemDictionaryManager.RetrieveItem(itemID) as Stock_Item;
        OnItemChanged.Invoke(Item);
    }

    public new void OnDestroy()
    {
        base.OnDestroy();
    }
}
