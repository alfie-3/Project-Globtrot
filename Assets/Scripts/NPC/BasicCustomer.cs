using NUnit.Framework;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEditor;
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

    public bool HasItemsInShoppingList => ShoppingList.Count > 0;

    public void RemoveItemFromShoppingList()
    {
        if (CurrentSearchingItem == null) return;

        ShoppingList.Remove(CurrentSearchingItem);
        return;
    }

    public bool TryGetShoppingListShelf(out StockShelvesManager shelf)
    {
        shelf = null;
        if (ShoppingList.Count <= 0) { return false; }

        if (ShelfStockTrackingManager.TryGetShelfRandom(CurrentSearchingItem.DesiredItem.ItemID, out StockShelvesManager shelfManager))
        {
            shelf = shelfManager;

            return true;
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

    public void OnDrawGizmosSelected()
    {
        if (CurrentSearchingItem != null)
        {
            Gizmos.color = Color.green;

            GUIStyle style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                richText = true
            };

            Handles.Label(transform.position + Vector3.up/2, $"<color=red> Looking for {CurrentSearchingItem.DesiredItem.ItemID}", style);
        }
    }
}

[System.Serializable]
public class ShoppingListItem
{
    public ShopProduct_Item DesiredItem;
    public int QuantityToPurchase;
}