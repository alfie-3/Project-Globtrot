using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NutrientBaby : NetworkBehaviour
{
    [SerializeField] List<Stock_Item> requestableStock;
    [SerializeField] Image brainScannerScreen;
    [Space]
    [SerializeField] Transform outputSpawnPoint;
    [SerializeField] NetworkObject energyCorePrefab;

    NetworkVariable<FixedString32Bytes> requestedItemID = new();
    Stock_Item requestedItem;

    public Action<bool> OnFeed = delegate { };
    public Action<Stock_Item> OnNewItemAssigned = delegate { };

    private void Awake() {
        requestedItemID.OnValueChanged += (prev, current) => AssignItem(current);
        OnNewItemAssigned += UpdateBrainScanner;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            GenerateNewItem();
        }
        else
        {
            AssignItem(requestedItemID.Value);
        }

    }

    public bool TryFeed(NetworkObjectReference feedingItem, Stock_Item inputItem) {
        if (!requestableStock.Contains(inputItem))
        {
            return false;
        }

        if (inputItem.ItemID.Equals(requestedItemID.Value.ToString())) {
            Feed_Rpc(feedingItem, true);
        }
        else {
            Feed_Rpc(feedingItem, false);
        }

        if (IsServer)
        {
            GenerateNewItem();
        }

        return true;
    }
    
    public bool CheckStock(Stock_Item item)
    {
        return requestableStock.Contains(item);
    }

    [Rpc(SendTo.Everyone)]
    public void Feed_Rpc(NetworkObjectReference feedingItem, bool success)
    {
        if (success)
        {
            OnFeed.Invoke(true);

            if (IsServer)
            {
                NetworkObject newNwObj = Instantiate(energyCorePrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
                newNwObj.Spawn();
            }
        }
        else
        {
            OnFeed.Invoke(false);
        }

        if (IsServer)
        {
            if (feedingItem.TryGet(out NetworkObject nwObj))
            {
                nwObj.Despawn();
            }
        }
    }

    public void GenerateNewItem() {
        Stock_Item newItem = requestableStock[UnityEngine.Random.Range(0, requestableStock.Count)];
        requestedItemID.Value = newItem.ItemID;
    }

    public void AssignItem(FixedString32Bytes current) {
        requestedItem = ItemDictionaryManager.RetrieveItem(current.ToString()) as Stock_Item;
        OnNewItemAssigned.Invoke(requestedItem);
    }

    public void UpdateBrainScanner(Stock_Item item) {
        if (!brainScannerScreen) return;

        brainScannerScreen.sprite = item.ItemIcon;
    }
}
