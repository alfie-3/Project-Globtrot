using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class Contents
{
    [SerializeField] int maxContentsAmount = 4;
    [SerializeField] bool useLimit = true;
    public bool AllowItems = true;

    public Dictionary<Stock_Item, int> ContentsDictionary { get; private set; } = new Dictionary<Stock_Item, int>();

    public int Count => ContentsDictionary.Count;

    public Contents()
    {
        ContentsDictionary = new Dictionary<Stock_Item, int>();
    }

    public Contents(int maxContentAmount)
    {
        ContentsDictionary = new Dictionary<Stock_Item, int>();

        if (maxContentAmount <= 0)
        {
            useLimit = false;
        }
        else
        {
            this.maxContentsAmount = maxContentAmount;
        }
    }

    public bool TryAddItem(Stock_Item item, int quantity = 1)
    {
        if (!AllowItems) return false;

        if (ContentsDictionary.Count > maxContentsAmount && useLimit) return false;

        if (ContentsDictionary.TryAdd(item, quantity))
        {
            return true;
        }

        ContentsDictionary[item] += quantity;

        return true;
    }

    public bool TryRemoveItem(string id, int quantity = 1)
    {
        if (ContentsDictionary.Count == 0) return false;

        Stock_Item item = (Stock_Item)ItemDictionaryManager.RetrieveItem(id);
        if (item == null) return false;

        if (ContentsDictionary.TryGetValue(item, out int value))
        {
            value -= quantity;

            if (value <= 0)
            {
                ContentsDictionary.Remove(item);
                return true;
            }

            return true;
        }

        return false;
    }
}

public interface IContents
{
    public Contents Contents { get; }
}