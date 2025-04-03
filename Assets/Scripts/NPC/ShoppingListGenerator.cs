using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class WeightedRandomBag<T>
{
    [System.Serializable]
    public struct Entry
    {
        public double accumulatedWeight;
        public T item;
    }

    private List<Entry> entries = new List<Entry>();
    private double accumulatedWeight;
    private System.Random rand = new System.Random();

    public void AddEntry(T item, double weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
    }

    public T GetRandom()
    {
       double r = rand.NextDouble() * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}

