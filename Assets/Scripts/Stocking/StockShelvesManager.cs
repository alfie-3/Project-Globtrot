using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class StockShelvesManager : NetworkBehaviour
{
    [field: SerializeField] public StockShelfControllerNew[] StockShelves {  get; private set; }
    Dictionary<string, StockedItemData> StockedItemInformation = new Dictionary<string, StockedItemData>();

    public static Action<string, string, StockShelvesManager> OnStockShelfUpdated = delegate { };
    public static Action<StockShelvesManager, string[]> OnStockShelfRemoved = delegate { };

    private void Awake()
    {
        foreach (StockShelfControllerNew shelf in StockShelves)
        {
            shelf.OnStockUpdated += UpdateStockTypesInformation;
        }
    }

    public int TakeItemsFromShelf(ShopProduct_Item product, int quantity)
    {
        int quantityToTake = quantity;

        foreach(var shelf in StockShelves)
        {
            //If shelf does not match product type return
            if (shelf.Holder.ItemId.Value != product.ItemID) continue;

            //If shelf has no items return
            if (shelf.Holder.ItemQuantity.Value == 0) continue;

            if (shelf.Holder.ItemQuantity.Value < quantityToTake)
            {
                quantityToTake -= shelf.Holder.ItemQuantity.Value;
                shelf.Holder.RemoveItem(shelf.Holder.ItemQuantity.Value);

                continue;
            }
            else
            {
                shelf.Holder.RemoveItem(quantity);

                return quantity;
            }
        }

        return quantity - quantityToTake;
    }

    public void UpdateStockTypesInformation(string previousStockType, string currentStockType, int quantity)
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

    public void OnDisable()
    {
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