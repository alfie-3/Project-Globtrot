using System.Collections.Generic;
using UnityEngine;

public static class StockPricingManager
{
    static Dictionary<string, ItemPricingData> ItemPricingDict;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        ItemPricingDict = new Dictionary<string, ItemPricingData>();
    }

    public static ItemPricingData GetPricingData(string itemID)
    {
        if (ItemPricingDict.ContainsKey(itemID))
        {
            return ItemPricingDict[itemID];
        }

        else
        {
            AddItemPricingData(itemID);
            return ItemPricingDict[itemID];
        }
    }

    public static ItemPricingData AddItemPricingData(string itemID)
    {
        ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (shopItem == null) return null;

        ItemPricingDict.TryAdd(itemID, new(shopItem.Pricing.BasePrice, shopItem.Pricing.BasePrice));

        return ItemPricingDict[itemID];
    }

    public static void SetSellPrice(string itemID, int newPrice)
    {
        ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (shopItem == null) return;

        if (ItemPricingDict.TryGetValue(itemID, out ItemPricingData itemPricingData))
        {
            itemPricingData.CurrentSellPrice = newPrice;
        }
        else
        {
            ItemPricingDict.TryAdd(itemID, new(shopItem.Pricing.BasePrice, newPrice));
        }
    }
}

public class ItemPricingData
{
    public int CurrentPurchasePrice;
    public int CurrentSellPrice;

    public ItemPricingData(int currentPurchasePrice, int currentSellPrice)
    {
        CurrentPurchasePrice = currentPurchasePrice;
        CurrentSellPrice = currentSellPrice;
    }
}