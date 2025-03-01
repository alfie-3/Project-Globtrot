using System;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class PriceTag : MonoBehaviour
{
    [SerializeField] StockShelfController referenceShelf;
    [SerializeField] TMP_Text text;

    private void OnEnable()
    {
        referenceShelf.OnStockUpdated += OnStockUpdated;

        text.text = "-";
    }

    private void OnDisable()
    {
        referenceShelf.OnStockUpdated -= OnStockUpdated;
    }

    public void OnStockUpdated(string prev, string current, int quantity)
    {
        if (current.IsNullOrEmpty())
        {
            text.text = "-";

            if (prev.IsNullOrEmpty()) return;

            ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(prev);
            if (shopItem == null) return;

            shopItem.Pricing.OnPriceUpdated -= UpdatePrice;
        }

        if (prev != current)
        {
            ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(current);
            if (shopItem == null) return;

            UpdatePrice(shopItem.GetCurrentSellPrice());
            shopItem.Pricing.OnPriceUpdated += UpdatePrice;
        }
    }

    public void UpdatePrice(int _, int currentPrice)
    {
        text.text = MoneyFormatter.FormatPriceInt(currentPrice);
    }

    public void UpdatePrice(int currentPrice)
    {
        text.text = MoneyFormatter.FormatPriceInt(currentPrice);
    }
}
