using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Product", menuName = "Items/Shop Product")]
public class ShopProduct_Item : ItemBase, IItemPrice
{
    [field: SerializeField] public ItemPrice Pricing { get; private set; }

    [field: SerializeField] public Sprite ProductImage { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public bool Stackable { get; private set; }
    [field: SerializeField] public int MaxInBox { get; private set; }
    [field: SerializeField] public ItemHolder.ContainerTypes ContanierCompatabilty { get; private set; }

    public int GetCurrentPurchasePrice()
    {
        return Pricing.GetCurrentPurchasePrice(ItemID);
    }

    public int GetCurrentSellPrice()
    {
        return Pricing.GetCurrentSellPrice(ItemID);
    }

    public void SetSellPrice(int newPrice)
    {
        Pricing.SetSellPrice(ItemID, newPrice);
    }
}
