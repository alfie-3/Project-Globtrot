using UnityEngine;
using TMPro;

// script for the ui obj that is added to the basket - represents product
public class UI_BasketProduct : MonoBehaviour
{
    [SerializeField] TMP_Text productAmount;
    [SerializeField] TMP_Text productName;
    [SerializeField] TMP_Text productPrice;
    [SerializeField] UI_Basket basketScript;
    private int amount = 1;
    private ItemBase itemData; 

    // updates ui to reflect product details
    public void UpdateProduct(ItemBase newItem, int newAmount)
    {
        itemData = newItem;
        productName.text = newItem.ItemName;
        amount = newAmount;
        productAmount.text = amount.ToString();


        if (itemData is ShopProduct_Item productItem)
        {
            productPrice.text = (productItem.GetCurrentPurchasePrice() * amount).ToString("F2");
        }
        else if (itemData is PlacableFurniture_Item furnitureItem)
        {
            productPrice.text = (furnitureItem.FurniturePrice * amount).ToString("F2");
        }
    }

    // update amount if multiple of same object
    public void IncreaseAmount()
    {
        amount++;
        productAmount.text = amount.ToString();

        // update total price when amount changes
        if (itemData is ShopProduct_Item productItem)
        {
            productPrice.text = (productItem.GetCurrentPurchasePrice() * amount).ToString("F2");
        }
        else if (itemData is PlacableFurniture_Item furnitureItem)
        {
            productPrice.text = (furnitureItem.FurniturePrice * amount).ToString("F2");
        }

        basketScript.UpdateTotal();
    }

    // sets reference to basket - so it can keep track
    public void SetShop(UI_Basket basket, string productID)
    {
        this.basketScript = basket;
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
        basketScript.RemoveItem(this);
    }
}
