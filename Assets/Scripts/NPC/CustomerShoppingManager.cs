using System.Collections.Generic;
using UnityEngine;

public class CustomerShoppingManager : MonoBehaviour
{
    public List<ShoppingListItem> ShoppingList;

    private void Awake()
    {
        ShoppingList = ShoppingListGenerator.GenerateShoppingList();
    }

    public bool HasItemsInShoppingList()
    {
        foreach(var item in ShoppingList)
        {
            if (!item.CheckedOff) return true;
        }

        return false;
    }

    public void TakeShoppingListItemsFromShelf(StockShelvesManager shelvesManager)
    {
        foreach (ShoppingListItem item in ShoppingList)
        {
            if (item.DesiredItem.GetCurrentSellPrice() > item.DesiredItem.GetCurrentPurchasePrice() * 2)
            {
                Debug.Log("Item too expensive");
                item.CheckOffListItem();
                continue;
            }

            int takenAmount = shelvesManager.TakeItemsFromShelf(item.DesiredItem, item.QuantityToPurchase);

            item.heldQuantity += takenAmount;

            item.ListItemValue += item.DesiredItem.GetCurrentSellPrice() * takenAmount;

            if (item.heldQuantity >= item.QuantityToPurchase)
                item.CheckOffListItem();
        }
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

    public void PurchaseItems()
    {
        foreach (var item in ShoppingList)
        {
            MoneyManager.Instance.AddMoney(item.heldQuantity * item.ListItemValue);
        }
    }

    private void OnDisable()
    {
        CustomerSpawnPoint.customersCount--;
    }
}

[System.Serializable]
public class ShoppingListItem
{
    public Stock_Item DesiredItem;

    public bool CheckedOff { get; private set; }

    public int QuantityToPurchase;
    public int heldQuantity;

    public double ListItemValue;

    public void CheckOffListItem()
    {
        CheckedOff = true;
    }

    public ShoppingListItem(Stock_Item desiredItem, int quantity)
    {
        DesiredItem = desiredItem;
        QuantityToPurchase = quantity;
    }
}