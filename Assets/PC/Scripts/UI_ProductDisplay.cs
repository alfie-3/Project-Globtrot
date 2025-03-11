using UnityEngine;
using UnityEngine.UI;
using TMPro;

// script for displaying buyable product
public class UI_ProductDisplay : MonoBehaviour
{
    [SerializeField] private Image productImage;
    [SerializeField] private TMP_Text productTitle;
    [SerializeField] private TMP_Text productPrice;
    [SerializeField] private TMP_Text quantityText;
    private UI_Basket basketScript;

    private ShopProduct_Item productData;

    private int quantity = 1;

    public void Initialize(ShopProduct_Item product, UI_Basket basket)
    {
        basketScript = basket;
        productData = product;
        SpawnProductUI();
        UpdateQuantityUI();

    }
    private void IncreaseQuantity()
    {
        quantity++;
        UpdateQuantityUI();
    }

    private void DecreaseQuantity()
    {
        if (quantity > 1)
        {
            quantity--;
            UpdateQuantityUI();
        }
    }

    private void UpdateQuantityUI()
    {
        quantityText.text = quantity.ToString();
    }

    private void SpawnProductUI()
    {
        productImage.sprite = productData.ItemIcon;
        productTitle.text = productData.ItemName;
        productPrice.text = $"${productData.GetCurrentPurchasePrice():F2}";
    }

    public void AddToBasket()
    {
        if (basketScript != null)
        {
            for (int i = 0; i < quantity; i++)
            {
                basketScript.SendToBasket(productData.ItemID);
            }
        }
        else
        {
            Debug.Log("no shop!");
        }
    }
}
