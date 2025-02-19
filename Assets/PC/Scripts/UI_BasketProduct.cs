using UnityEngine;
using TMPro;

// script for the ui obj that is added to the basket - represents product
public class UI_BasketProduct : MonoBehaviour
{
    [SerializeField] TMP_Text productAmount;
    [SerializeField] TMP_Text productName;
    [SerializeField] TMP_Text productPrice;
    private UI_StockShop shopScript;
    private int amount = 1;

    // updates ui to reflect product details
    public void UpdateProduct(string newName, int newAmount)
    {
        productName.text = newName;
        amount = newAmount;
        productAmount.text = amount.ToString();

        if (ItemDictionaryManager.ItemDict.TryGetValue(newName, out ItemBase item) && item is ShopProduct_Item productItem)
        {
            productPrice.text = (productItem.Price * amount).ToString("F2");
        }
    }

    // update amount if multiple of same object
    public void IncreaseAmount()
    {
        amount++;
        productAmount.text = amount.ToString();

        // update total price when amount changes
        if (ItemDictionaryManager.ItemDict.TryGetValue(productName.text, out ItemBase item) && item is ShopProduct_Item productItem)
        {
            productPrice.text = (productItem.Price * amount).ToString("F2");
            shopScript.UpdateTotal();
        }
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
