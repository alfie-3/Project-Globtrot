using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Product", menuName = "Items/Shop Product")]
public class Stock_Item : ItemBase
{
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float TimeContribution { get; private set; } = 5f;
    [field: SerializeField] private List<WeightedProductSelectionItem> weightedRandomCustomerPickupChance;
    public WeightedRandomBag<int> WeightedQuantitySelection = new();
    [field: Space]
    [field: SerializeField] public float SalePercentage = 0.25f;
    [field: SerializeField] public float UnlockPrice = 50.0f;
    [field: SerializeField] public bool Unlockable = false;

    public int GetCurrentPrice()
    {
        return Price;
    }

    public override void Init()
    {
        base.Init();

        foreach (WeightedProductSelectionItem weightedProductSelectionItem in weightedRandomCustomerPickupChance)
        {
            WeightedQuantitySelection.AddEntry(weightedProductSelectionItem.Quantity, weightedProductSelectionItem.Weight);
        }
    }
}

[System.Serializable]
public struct WeightedProductSelectionItem
{
    public int Quantity;
    public double Weight;

    public WeightedProductSelectionItem(int quantity, double weight)
    {
        Quantity = quantity;
        Weight = weight;
    }
}