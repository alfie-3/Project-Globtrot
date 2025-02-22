using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(ItemHolder))]
public class StockShelfControllerNew : NetworkBehaviour {

    //public Action<string, string, int> OnStockUpdated = delegate {};
    [SerializeField] private ItemHolder holder;

    private Stack<NetworkObject> items = new Stack<NetworkObject>();
    public Action<string, string, int> OnStockUpdated = delegate { };

    private int maxItems;

    private Vector3 itemStackBounds;
    private Vector3 itemStackOffset;
    private Bounds itemBounds;

    private void Awake()
    {
        holder.OnStockUpdated += UpdateShelf;
    }

    public void UpdateShelf(string previousStockType, string currentStockType, int quantity)
    {
        if (!previousStockType.Equals(currentStockType) && !currentStockType.IsNullOrEmpty())
        {
            ShopProduct_Item item = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(currentStockType);
            itemBounds = item.Prefab.GetComponent<MeshFilter>().sharedMesh.bounds;
            CalculateSpawnPositioning(item.Stackable);
            holder.SetMaxItems((int)(itemStackBounds.x * itemStackBounds.y * itemStackBounds.z));
        }
        if (quantity > items.Count)
            SpawnItem_RPC(currentStockType, quantity - items.Count);
        else
            RemoveItem_RPC(items.Count - quantity);

        OnStockUpdated.Invoke(previousStockType, currentStockType, holder.ItemQuantity.Value);
    }

    [Rpc(SendTo.Server)]
    void SpawnItem_RPC(string itemid, int quanitity) {
        ShopProduct_Item placeableItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemid);
        if (placeableItem == null) return;
        Debug.Log($"Placing Stock item {itemid} {quanitity} times");
        NetworkObject instance;

        for (int i = 0; i < quanitity; i++)
        {
            instance = Instantiate(placeableItem.Prefab, Vector3.Scale(GetSpawnPos(items.Count), itemBounds.size) + transform.position + itemStackOffset, Quaternion.identity).GetComponent<NetworkObject>();
            instance.Spawn();
            instance.TrySetParent(transform.parent.GetComponent<NetworkObject>());
            items.Push(instance);
        }
    }

    [Rpc(SendTo.Server)]
    public void RemoveItem_RPC(int quanitity)
    {
        for (int i = 0; i < quanitity; i++)
            items.Pop().Despawn();
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
