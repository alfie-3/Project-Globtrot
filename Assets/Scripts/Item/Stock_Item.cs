using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Product", menuName = "Items/Shop Product")]
public class Stock_Item : ItemBase, IItemPrice
{
    [field: SerializeField] public ItemPrice Pricing { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: Space]
    [field: SerializeField] public bool Stackable { get; private set; }
    [field: SerializeField] public int MaxInBox { get; private set; }
    [field: SerializeField] public float TimeContribution { get; private set; } = 5f;
    [field: SerializeField] private List<WeightedProductSelectionItem> weightedRandomCustomerPickupChance;
    public WeightedRandomBag<int> WeightedQuantitySelection = new();
    [field: Space]
    [field: SerializeField] public ItemHolder.ContainerTypes ContanierCompatabilty { get; private set; }
    [field: SerializeField] public ProductCategory Category { get; private set; } 
    [field: SerializeField] public float SalePercentage = 0.25f;
    [field: SerializeField] public float UnlockPrice = 50.0f;
    [field: SerializeField] public bool Unlockable = false;



    /// <summary>
    /// Price that PLAYERS purchase products at.
    /// </summary>
    /// <returns></returns>
    public double GetCurrentPurchasePrice()
    {
        double price = Pricing.GetCurrentPurchasePrice(ItemID);
        price = price * MaxInBox;
        price = price - (price* SalePercentage);
        return price;
    }

    /// <summary>
    /// Price that CUSTOMERS purchase products at.
    /// </summary>
    /// <returns></returns>
    public double GetCurrentSellPrice()
    {
        return Pricing.GetCurrentSellPrice(ItemID);
    }

    public void SetSellPrice(double newPrice)
    {
        Pricing.SetSellPrice(ItemID, newPrice);
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