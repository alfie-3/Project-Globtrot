using UnityEngine;
using TMPro;

// script for the ui obj that is added to the basket - represents product
public class UI_BasketProduct : MonoBehaviour
{
    [SerializeField] TMP_Text productAmount;
    [SerializeField] TMP_Text productName;
    private UI_StockShop shopScript;
    private int amount = 1;

    // updates ui to reflect product name + amount
    public void UpdateProduct(string newName, int newAmount)
    {
        productName.text = newName;
        amount = newAmount;
        productAmount.text = amount.ToString();
    }

    // sets reference to shop - so it can keep track
    public void SetShop(UI_StockShop shop, string productID)
    {
        this.shopScript = shop;
        this.productName.text = productID;
    }

    public string GetProductName()
    {
        return productName.text;
    }

    public int GetAmount()
    {
        return amount;
    }

    public void Trash()
    {
        shopScript.RemoveItem(this);
    }
}
