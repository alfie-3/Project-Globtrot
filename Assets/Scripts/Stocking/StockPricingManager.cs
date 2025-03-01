using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StockPricingManager : NetworkBehaviour
{
    static Dictionary<string, ItemPricingData> ItemPricingDict;
    public static StockPricingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        ItemPricingDict = new Dictionary<string, ItemPricingData>();
        Instance = null;
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

    public static void SetSellPrice(string itemID, double newPrice)
    {
        ShopProduct_Item shopItem = (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemID);
        if (shopItem == null) return;

        if (ItemPricingDict.TryGetValue(itemID, out ItemPricingData itemPricingData))
        {
            double oldPrice = itemPricingData.CurrentSellPrice;
            itemPricingData.CurrentSellPrice = newPrice;

            shopItem.Pricing.OnPriceUpdated.Invoke(oldPrice, newPrice);
        }
        else
        {
            ItemPricingDict.TryAdd(itemID, new(shopItem.Pricing.BasePrice, newPrice));

            shopItem.Pricing.OnPriceUpdated.Invoke(shopItem.Pricing.BasePrice, newPrice);
        }
    }

    public void ReplicatePricingInformation(string itemId)
    {
        if (ItemPricingDict.TryGetValue(itemId, out ItemPricingData itemPricingData))
        {
            ReplicatePricingInformation_Rpc(itemId, itemPricingData.CurrentSellPrice);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ReplicatePricingInformation_Rpc(string itemId, double newPrice)
    {
        SetSellPrice(itemId, newPrice);
    }
}

public class ItemPricingData
{
    public double CurrentPurchasePrice;
    public double CurrentSellPrice;

    public ItemPricingData(double currentPurchasePrice, double currentSellPrice)
    {
        CurrentPurchasePrice = currentPurchasePrice;
        CurrentSellPrice = currentSellPrice;
    }
}