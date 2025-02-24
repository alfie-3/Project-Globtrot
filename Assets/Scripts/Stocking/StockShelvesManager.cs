using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class StockShelvesManager : NetworkBehaviour
{
    [field: SerializeField] public StockShelfController[] StockShelves {  get; private set; }
    Dictionary<string, StockedItemData> StockedItemInformation = new Dictionary<string, StockedItemData>();

    public static Action<string, string, StockShelvesManager> OnStockShelfUpdated;
    public static Action<StockShelvesManager, string[]> OnStockShelfRemoved;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        OnStockShelfUpdated = delegate { };
        OnStockShelfRemoved = delegate { };
    }

    private void Awake()
    {
        foreach (StockShelfController shelf in StockShelves)
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
        if (previousStockType.IsNullOrEmpty())
        {
            if (StockedItemInformation.TryGetValue(currentStockType, out StockedItemData stockedItemData))
            {
                int difference = quantity - stockedItemData.ItemQuanitity;
                stockedItemData.ItemQuanitity += difference;
            }
            else
            {
                StockedItemData newStockedItemData = new StockedItemData()
                {
                    ItemID = currentStockType,
                    ItemQuanitity = quantity
                };

                StockedItemInformation.Add(currentStockType, newStockedItemData);
            }
        }

        else if (previousStockType.Equals(currentStockType))
        {
            if (StockedItemInformation.TryGetValue(currentStockType, out StockedItemData stockedItemData))
            {
                int difference = quantity - stockedItemData.ItemQuanitity;
                stockedItemData.ItemQuanitity += difference;

                if (stockedItemData.ItemQuanitity <= 0)
                {
                    StockedItemInformation.Remove(currentStockType);
                }
            }
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

public class StockedItemData
{
    public string ItemID;
    public int ItemQuanitity;
}