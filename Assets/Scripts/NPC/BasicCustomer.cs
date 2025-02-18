using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicCustomer : MonoBehaviour
{
    public List<ShoppingListItem> ShoppingList = new List<ShoppingListItem>();

    public ShoppingListItem CurrentSearchingItem;

    private void Awake()
    {
        if (ShoppingList.Count <= 0) return;

        CurrentSearchingItem = ShoppingList[Random.Range(0, ShoppingList.Count)];
    }

    public bool TryGetShoppingListShelf(out GameObject shelf)
    {
        shelf = null;
        if (ShoppingList.Count <= 0) { return false; }

        if (ShelfStockTrackingManager.TryGetShelfRandom(CurrentSearchingItem.DesiredItem.ItemID, out StockShelvesManager shelfManager))
        {
            ShoppingList.Remove(CurrentSearchingItem);
            shelf = shelfManager.gameObject;

            if (ShoppingList.Count > 0)
                CurrentSearchingItem = ShoppingList[Random.Range(0, ShoppingList.Count)];

            return true;
        }

        return false;
    }
}

[System.Serializable]
public class ShoppingListItem
{
    public ShopProduct_Item DesiredItem;
    public int QuantityToPurchase;
}