using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

[RequireComponent(typeof(ItemHolder))]
public class StockShelfController : NetworkBehaviour {

    //public Action<string, string, int> OnStockUpdated = delegate {};
    [field: SerializeField] public ItemHolder Holder { get; private set; }

    private Stack<NetworkObject> items = new Stack<NetworkObject>();
    public Action<string, string, int> OnStockUpdated = delegate { };

    private int maxItems;

    private Vector3 itemStackBounds;
    private Vector3 itemStackOffset;
    private Bounds itemBounds;

    private void Awake()
    {
        Holder.OnStockUpdated += UpdateShelf;
    }

    public void UpdateShelf(string previousStockType, string currentStockType, int quantity)
    {
        if (!previousStockType.Equals(currentStockType) && !currentStockType.IsNullOrEmpty())
        {
            ShopProduct_Item item = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(currentStockType);
            itemBounds = item.Prefab.GetComponent<MeshFilter>().sharedMesh.bounds;
            CalculateSpawnPositioning(item.Stackable);
            Holder.SetMaxItems((int)(itemStackBounds.x * itemStackBounds.y * itemStackBounds.z));
        }
        if (quantity > items.Count)
            SpawnItem_RPC(currentStockType, quantity - items.Count);
        else
            RemoveItem_RPC(items.Count - quantity);

        OnStockUpdated.Invoke(previousStockType, currentStockType, Holder.ItemQuantity.Value);
    }

    public void SetItemPricing(double desiredPrice)
    {
        if (Holder.IsEmpty) return;

        Holder.ProductItem.SetSellPrice(desiredPrice);
    }

    [Rpc(SendTo.Server)]
    void SpawnItem_RPC(string itemid, int quanitity) {
        ShopProduct_Item placeableItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemid);
        if (placeableItem == null) return;
        Debug.Log($"Placing Stock item {itemid} {quanitity} times");
        NetworkObject instance;

        for (int i = 0; i < quanitity; i++)
        {
            instance = Instantiate(placeableItem.Prefab, transform.TransformPoint(Vector3.Scale(GetSpawnPos(items.Count), itemBounds.size) + itemStackOffset), Quaternion.identity).GetComponent<NetworkObject>();
            instance.Spawn();
            instance.TrySetParent(transform.parent.GetComponent<NetworkObject>());
            //instance.transform.position = 
            Debug.Log(itemStackOffset);
            items.Push(instance);
        }
    }

    [Rpc(SendTo.Server)]
    public void RemoveItem_RPC(int quantity)
    {
        for (int i = 0; i < quantity; i++)
            items.Pop().Despawn();
    }

    private Vector3 GetSpawnPos(int index)
    {
        Vector3 spawnPos = new();
        spawnPos.x = (int)(index % itemStackBounds.x);
        spawnPos.z = ((index / (int)itemStackBounds.x) % (int)itemStackBounds.z);
        spawnPos.y = index / ((int)itemStackBounds.x * (int)itemStackBounds.z);
        Debug.Log(spawnPos);
        return spawnPos;
    }

    private void CalculateSpawnPositioning(bool stackable)
    {
        Vector3 shelfBounds = GetComponent<SimpleVolume>().m_Size;

        itemStackBounds.x = (float)Math.Floor((double)(shelfBounds.x / itemBounds.size.x));
        itemStackBounds.y = stackable ? (float)Math.Floor((double)(shelfBounds.y / itemBounds.size.y)) : 1;
        itemStackBounds.z = (float)Math.Floor((double)(shelfBounds.z / itemBounds.size.z));


        Vector3 EvenOutOffset = Vector3.zero;
        EvenOutOffset.x = (shelfBounds.x % itemBounds.size.x) / 2;
        EvenOutOffset.z = (shelfBounds.z % itemBounds.size.z) / 2;

        itemStackOffset = ((itemBounds.size * 0.5f) - itemBounds.center) + EvenOutOffset;

    }
}
