using System.Collections.Generic;
using UnityEngine;

public class BasicCustomer : MonoBehaviour
{
    public List<ShoppingListItem> ShoppingList = new List<ShoppingListItem>();

    private void Awake()
    {
        if (ShoppingList.Count <= 0) return;
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
        for (int i = 0; i < ShoppingList.Count; i++)
        {
            int takenAmount = shelvesManager.TakeItemsFromShelf(ShoppingList[i].DesiredItem, ShoppingList[i].QuantityToPurchase);

            ShoppingList[i].heldQuantity += takenAmount;

            ShoppingList[i].ListItemValue += ShoppingList[i].DesiredItem.GetCurrentSellPrice() * takenAmount;

            if (ShoppingList[i].heldQuantity >= ShoppingList[i].QuantityToPurchase)
                ShoppingList[i].CheckOffListItem();
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
    public ShopProduct_Item DesiredItem;

    public bool CheckedOff { get; private set; }

    public int QuantityToPurchase;
    public int heldQuantity;

    public double ListItemValue;

    public void CheckOffListItem()
    {
        CheckedOff = true;
    }
}