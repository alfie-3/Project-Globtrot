using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BasicCustomer : MonoBehaviour
{
    public List<ShoppingListItem> ShoppingList;
}

[System.Serializable]
public struct ShoppingListItem
{
    public ShopProduct_Item DesiredItem;
    public int QuantityToPurchase;
}