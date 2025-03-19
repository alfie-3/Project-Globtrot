using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OrderContainerBox : NetworkBehaviour, IContents
{
    [SerializeField] Contents boxContents;
    public Contents Contents => boxContents;
}

[System.Serializable]
public class Contents
{
    [SerializeField] int maxContentsAmount = 4;
    bool useLimit = true;

    public Dictionary<string, int> ContentsDictionary { get; private set; } = new Dictionary<string, int>();

    public int Count => ContentsDictionary.Count;

    public Contents()
    {
        ContentsDictionary = new Dictionary<string, int>();
    }

    public Contents(int maxContentAmount)
    {
        ContentsDictionary = new Dictionary<string, int>();

        if (maxContentAmount <= 0)
        {
            useLimit = false;
        }
        else
        {
            this.maxContentsAmount = maxContentAmount;
        }
    }

    public bool TryAddItem(string id, int quantity = 1)
    {
        if (ContentsDictionary.Count > maxContentsAmount && useLimit) return false;

        if (ContentsDictionary.TryAdd(id, quantity)) return true;

        ContentsDictionary[id] += quantity;
        return true;
    }

    public bool TryRemoveItem(string name, int quantity = 1)
    {
        if (ContentsDictionary.Count == 0) return false;

        if (ContentsDictionary.TryGetValue(name, out int value))
        {
            value -= quantity;

            if (value <= 0)
                ContentsDictionary.Remove(name);

            return true;
        }

        return false;
    }
}

public interface IContents
{
    public Contents Contents { get; }
}