using System;
using UnityEngine;

[System.Serializable]
public class ItemPrice
{
    [field: SerializeField] public int BasePrice { get; private set; }

    public int GetCurrentSellPrice(string id)
    {
        return StockPricingManager.GetPricingData(id).CurrentSellPrice;
    }

    public int GetCurrentPurchasePrice(string id)
    {
        return StockPricingManager.GetPricingData(id).CurrentPurchasePrice;
    }

    public void SetSellPrice(string id, int newPrice)
    {
        int oldPrice = GetCurrentSellPrice(id);

        StockPricingManager.SetSellPrice(id, newPrice);

        OnPriceUpdated.Invoke(oldPrice, newPrice);
    }

    public Action<int, int> OnPriceUpdated;
}

public interface IItemPrice
{
    public int GetCurrentPurchasePrice();

    public int GetCurrentSellPrice();

    public void SetSellPrice(int newPrice);
}