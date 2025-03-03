using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public static class ShelfStockTrackingManager
{
    static Dictionary<string, List<StockShelvesManager>> StockLookupDictionary;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        StockLookupDictionary = new();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterLoad()
    {
        StockShelvesManager.OnStockShelfUpdated = UpdateShelfInfo;
        StockShelvesManager.OnStockShelfRemoved = RemoveShelf;
    }

    public static bool TryGetShelfRandom(string ItemID, out StockShelvesManager stockShelf)
    {
        stockShelf = null;

        if (StockLookupDictionary.TryGetValue(ItemID, out List<StockShelvesManager> stockShelves))
        {
            if (stockShelves.Count == 0) return false;

            stockShelf = stockShelves[Random.Range(0, stockShelves.Count)];
            return true;
        }

        return false;
    }

    public static bool TryGetShelfClosest(string ItemID, Vector3 position, out StockShelvesManager closestStockShelf)
    {
        closestStockShelf = null;
        float closestDistance = 10000000000;

        if (StockLookupDictionary.TryGetValue(ItemID, out List<StockShelvesManager> stockShelves))
        {
            float currentDistance;

            foreach (StockShelvesManager stockShelf in stockShelves)
            {
                currentDistance = Vector3.Distance(stockShelf.transform.position, position);

                if (currentDistance < closestDistance)
                {
                    closestStockShelf = stockShelf;
                    closestDistance = currentDistance;
                }
            }

            return true;
        }

        return false;
    }

    public static void UpdateShelfInfo(string previousStockId, string currentStockId, StockShelvesManager stockShelf)
    {
        if (currentStockId.IsNullOrEmpty())
        {
            if (StockLookupDictionary.TryGetValue(previousStockId, out List<StockShelvesManager> stockShelves))
            {
                stockShelves.Remove(stockShelf);
            }
        }

        if (previousStockId.IsNullOrEmpty())
        {
            StockLookupDictionary.TryAdd(currentStockId, new List<StockShelvesManager>());

            StockLookupDictionary[currentStockId].Add(stockShelf);
        }
    }

    public static void RemoveShelf(StockShelvesManager stockShelf, params string[] currentStockIds)
    {
        foreach (string currentStockId in currentStockIds)
        {
            if (StockLookupDictionary.TryGetValue(currentStockId, out List<StockShelvesManager> stockShelves))
            {
                stockShelves.Remove(stockShelf);

                if (stockShelves.Count == 0)
                {
                    StockLookupDictionary.Remove(currentStockId);
                }
            }
        }
    }
}
