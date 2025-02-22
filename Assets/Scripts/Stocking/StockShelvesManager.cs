using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class StockShelvesManager : NetworkBehaviour
{
    [field: SerializeField] public ItemHolder[] StockShelves {  get; private set; }
    Dictionary<string, StockedItemData> StockedItemInformation = new Dictionary<string, StockedItemData>();

    public static Action<string, string, StockShelvesManager> OnStockShelfUpdated = delegate { };
    public static Action<StockShelvesManager, string[]> OnStockShelfRemoved = delegate { };

    private void Awake()
    {
        foreach (ItemHolder shelf in StockShelves)
        {
            shelf.OnStockUpdated += UpdateStockTypes;
        }
    }

    public void UpdateStockTypes(string previousStockType, string currentStockType, int quantity)
    {
        if (currentStockType.IsNullOrEmpty())
        {
            StockedItemInformation.Remove(previousStockType);
        }

        else if (previousStockType.IsNullOrEmpty())
        {
            StockedItemData stockedItemData = new StockedItemData()
            {
                ItemID = currentStockType,
                ItemQuanitity = quantity
            };

            StockedItemInformation.TryAdd(currentStockType, stockedItemData);
        }

        else if (previousStockType.Equals(currentStockType))
        {
            if (StockedItemInformation.TryGetValue(currentStockType, out StockedItemData stockedItemData))
            {
                stockedItemData.ItemQuanitity = quantity;            }
        }

        OnStockShelfUpdated.Invoke(previousStockType, currentStockType, this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        HashSet<string> storedItemTypes = new();

        foreach(string storedItemId in StockedItemInformation.Keys)
        {
            storedItemTypes.Add(storedItemId);
        }

        OnStockShelfRemoved.Invoke(this, storedItemTypes.ToArray());
    }
}

public struct StockedItemData
{
    public string ItemID;
    public int ItemQuanitity;
}