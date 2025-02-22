using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class StockShelfController : NetworkBehaviour {    
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool IsEmpty { get { return ItemQuantity.Value == 0; } }

    public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public Action<string, string, int> OnStockUpdated = delegate {};

    private Stack<NetworkObject> items = new Stack<NetworkObject>();

    private int maxItems;

    private Vector3 itemStackBounds;
    private Vector3 itemStackOffset;
    private Bounds itemBounds;


    [Rpc(SendTo.Server)]
    public void AddItemServer_Rpc(string itemId, int quanitity = 1)
    {
        Debug.Log("addeing item");
        if (IsEmpty)
        {
            if (ItemDictionaryManager.RetrieveItem(itemId.ToString()) is not ShopProduct_Item) return;
            SetItem(itemId);
        }
        if (ItemId.Value.ToString() == itemId)
        {
            for (int i = 0; i < quanitity; i++)
                SpawnItem_RPC();
            OnStockUpdated.Invoke(itemId, itemId, ItemQuantity.Value);
        }

    }

    [Rpc(SendTo.Server)]
    void SpawnItem_RPC() {
        if (ItemQuantity.Value == maxItems) return;
        ShopProduct_Item placeableItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(ItemId.Value.ToString());
        Debug.Log($"Placing Stock item {ItemId.Value.ToString()}");


        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.Prefab, Vector3.Scale(GetSpawnPos(ItemQuantity.Value),itemBounds.size) + transform.position + itemStackOffset, Quaternion.identity).GetComponent<NetworkObject>();
        instance.Spawn();
        instance.TrySetParent(transform.parent.GetComponent<NetworkObject>());
        items.Push(instance);

        ItemQuantity.Value++;
    }

    public bool RemoveItem()
    {
        if(IsEmpty) return false;
        RemoveItem_RPC();
        if (ItemQuantity.Value <= 0)
        {
            ClearItem();
        }
        return true;
    }

    [Rpc(SendTo.Server)]
    void RemoveItem_RPC()
    {
        ItemQuantity.Value--;
        items.Pop().Despawn();
    }



    public void ClearItem()
    {
        OnStockUpdated.Invoke(ItemId.Value.ToString(), string.Empty, 0);
        ItemId.Value = String.Empty;
    }

    public void SetItem(string itemId)
    {
        ItemId.Value = itemId;

        ShopProduct_Item item = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(ItemId.Value.ToString());
        itemBounds = item.Prefab.GetComponent<MeshFilter>().sharedMesh.bounds;
        CalculateSpawnPositioning(item.Stackable);
        maxItems = (int)(itemStackBounds.x *itemStackBounds.y * itemStackBounds.z);
        
        OnStockUpdated.Invoke(string.Empty, itemId, ItemQuantity.Value);
    }

    private Vector3 GetSpawnPos(int index)
    {
        Vector3 spawnPos = new();
        spawnPos.x = (int)(index % itemStackBounds.x);
        spawnPos.z = (int)((index / (int)itemStackBounds.x) % (int)itemStackBounds.z);
        spawnPos.y = index / ((int)itemStackBounds.x * (int)itemStackBounds.z);
        return spawnPos;
    }

    private void CalculateSpawnPositioning(bool stackable)
    {
        Bounds shelfBounds = GetComponent<MeshFilter>().sharedMesh.bounds;

        itemStackBounds.x = (float)Math.Floor((double)(shelfBounds.size.x / itemBounds.size.x));
        itemStackBounds.y = stackable ? (float)Math.Floor((double)(shelfBounds.size.y / itemBounds.size.y)) : 1;
        itemStackBounds.z = (float)Math.Floor((double)(shelfBounds.center.z / itemBounds.size.z));


        Vector3 EvenOutOffset = Vector3.zero;
        EvenOutOffset.x = (shelfBounds.size.x % itemBounds.size.x) / 2;
        EvenOutOffset.z = (shelfBounds.center.z % itemBounds.size.z) / 2;

        itemStackOffset = ((itemBounds.size * 0.5f) - itemBounds.center) + EvenOutOffset;

    }
}
