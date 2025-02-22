using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicCustomer : MonoBehaviour
{
    public List<ShoppingListItem> ShoppingList = new List<ShoppingListItem>();

    private void Awake()
    {
        if (ShoppingList.Count <= 0) return;
    }

    public bool HasItemsInShoppingList => ShoppingList.Count > 0;

    public void TakeShoppingListItemsFromShelf(StockShelvesManager shelvesManager)
    {
        for (int i = 0; i < ShoppingList.Count; i++)
        {
            int takenAmount = shelvesManager.TakeItemsFromShelf(ShoppingList[i].DesiredItem, ShoppingList[i].QuantityToPurchase);
            ShoppingList[i].QuantityToPurchase -= takenAmount;

            if (ShoppingList[i].QuantityToPurchase <= 0)
                RemoveItemFromShoppingList(ShoppingList[i]);
        }
    }

    public void RemoveItemFromShoppingList(ShoppingListItem referenceItem)
    {
        if (referenceItem == null) return;

        ShoppingList.Remove(referenceItem);
        return;
    }

    public bool TryGetShoppingListShelf(out StockShelvesManager shelf)
    {
        shelf = null;
        if (ShoppingList.Count <= 0) { return false; }

        foreach (var item in ShoppingList)
        {
            if (ShelfStockTrackingManager.TryGetShelfRandom(item.DesiredItem.ItemID, out StockShelvesManager shelfManager))
            {
                shelf = shelfManager;

                return true;
            }

            return false;
        }

        return false;
    }

    public bool TryGetCashRegister(out CashRegister cashRegister)
    {
        if (CashRegister.TryGetRandomCashRegister(out CashRegister register))
        {
            cashRegister = register;
            return true;
        }

        cashRegister = null;
        return false;
    }
}

[System.Serializable]
public class ShoppingListItem
{
    public ShopProduct_Item DesiredItem;
    public int QuantityToPurchase;
}