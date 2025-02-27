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
        referenceShelf.OnStockUpdated += UpdateText;

        text.text = "-";
    }

    private void OnDisable()
    {
        referenceShelf.OnStockUpdated -= UpdateText;
    }

    public void UpdateText(string prev, string current, int quantity)
    {
        if (current.IsNullOrEmpty())
        {
            text.text = "-";
        }

        if (prev != current)
        {
            ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(current);
            if (shopItem == null) return;

            text.text = MoneyFormatter.FormatPriceInt(shopItem.Price);
        }
    }
}
