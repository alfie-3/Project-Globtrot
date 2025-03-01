using System;
using UnityEngine;

[System.Serializable]
public class ItemPrice
{
    [field: SerializeField] public double BasePrice { get; private set; }

    public double GetCurrentSellPrice(string id)
    {
        return StockPricingManager.GetPricingData(id).CurrentSellPrice;
    }

    public double GetCurrentPurchasePrice(string id)
    {
        return StockPricingManager.GetPricingData(id).CurrentPurchasePrice;
    }

    public void SetSellPrice(string id, double newPrice)
    {
        StockPricingManager.SetSellPrice(id, newPrice);
        StockPricingManager.Instance.ReplicatePricingInformation(id);
    }

    public Action<double, double> OnPriceUpdated;
}

public interface IItemPrice
{
    public double GetCurrentPurchasePrice();

    public double GetCurrentSellPrice();

    public void SetSellPrice(double newPrice);
}