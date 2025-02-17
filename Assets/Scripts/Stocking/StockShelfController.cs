using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class StockShelfController : NetworkBehaviour {
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool IsEmpty { get
        { return ItemId.Value.IsEmpty; }}
    public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);



    public void AddItem(string itemId)
    {
        Debug.Log("HAHHA");
        Debug.Log(itemId);
        Debug.Log(IsEmpty);
        if (IsEmpty)
            SetItem(itemId);
        Debug.Log(ItemId.Value.ToString());
        if (ItemId.Value.ToString().Equals(itemId)) {
            ItemQuantity.Value++;
            SpawnItem_RPC();
        }
        
    }

    [Rpc(SendTo.Server)]
    void SpawnItem_RPC() {
        ShopProduct_Item placeableItem = ItemDictionaryManager.RetrieveItem(ItemId.Value.ToString()) is not ShopProduct_Item ? null : (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(ItemId.Value.ToString());
        Debug.Log($"Placing Stock item {ItemId.Value.ToString()}");
        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.Prefab, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        //instance.Spawn();

    }

    public void RemoveItem(string itemId)
    {
        if (itemId != ItemId.Value) return;

        ItemQuantity.Value--;

        if (ItemQuantity.Value <= 0)
        {
            ClearItem();
        }
    }

    public void ClearItem()
    {
        ItemId.Value = null;
        //IsEmpty = true;
    }

    public void SetItem(string itemId)
    {
        ItemId.Value = itemId;
        //IsEmpty = false;
    }
}
