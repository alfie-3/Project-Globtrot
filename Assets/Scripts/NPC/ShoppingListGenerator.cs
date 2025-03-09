using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShoppingListGenerator
{
    public static List<ShopProduct_Item> AvailableProducts;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        AvailableProducts = new();

        foreach(ItemBase item in ItemDictionaryManager.ItemDict.Values)
        {
            if (item is ShopProduct_Item)
            {
                AvailableProducts.Add(item as ShopProduct_Item);
            }
        }
    }

    public static List<ShoppingListItem> GenerateShoppingList()
    {
        List<ShoppingListItem> shoppingList = new();

        ShopProduct_Item item = AvailableProducts[Random.Range(0, AvailableProducts.Count)];
        int quantity = 1;

        int weightedSum = 0;

        foreach (WeightedProductSelectionItem weightedQuantity in item.WeightedQuantitySelection)
        {
            weightedSum += weightedQuantity.Entries;
        }

        int randomSelected = Random.Range(0, weightedSum);

        quantity = item.WeightedQuantitySelection.FirstOrDefault(e => weightedSum >= randomSelected).Quantity;

        shoppingList.Add(new(item, quantity));

        return shoppingList;
    }
}
