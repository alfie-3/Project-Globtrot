using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

public class ShelfStockTrackingManager : MonoBehaviour
{
    static Dictionary<string, HashSet<StockShelvesManager>> StockLookupDictionary;
    static System.Random random;

    public void Awake()
    {
        StockLookupDictionary = new();
        random = new System.Random();

        StockShelvesManager.OnStockShelfUpdated = UpdateShelfInfo;
        StockShelvesManager.OnStockShelfRemoved = RemoveShelf;
    }

    public static bool TryGetShelfRandom(string ItemID, out StockShelvesManager stockShelf)
    {
        stockShelf = null;

        if (StockLookupDictionary.TryGetValue(ItemID, out HashSet<StockShelvesManager> stockShelves))
        {
            if (stockShelves.Count == 0) return false;

            stockShelf = stockShelves.ElementAt(random.Next(stockShelves.Count));
            return true;
        }

        return false;
    }

    public static bool TryGetShelfClosest(string ItemID, Vector3 position, out StockShelvesManager closestStockShelf)
    {
        closestStockShelf = null;
        float closestDistance = 10000000000;

        if (StockLookupDictionary.TryGetValue(ItemID, out HashSet<StockShelvesManager> stockShelves))
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
            if (StockLookupDictionary.TryGetValue(previousStockId, out HashSet<StockShelvesManager> stockShelves))
            {
                stockShelves.Remove(stockShelf);
            }
        }

        if (previousStockId.IsNullOrEmpty())
        {
            StockLookupDictionary.TryAdd(currentStockId, new HashSet<StockShelvesManager>());

            StockLookupDictionary[currentStockId].Add(stockShelf);
        }
    }

    public static void RemoveShelf(StockShelvesManager stockShelf, params string[] currentStockIds)
    {
        foreach (string currentStockId in currentStockIds)
        {
            if (StockLookupDictionary.TryGetValue(currentStockId, out HashSet<StockShelvesManager> stockShelves))
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
