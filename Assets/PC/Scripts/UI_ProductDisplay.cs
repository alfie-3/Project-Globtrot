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
    [SerializeField] private ProductData productData;

    private UI_StockShop ShopScript;
    private int quantity = 1;

    private void Start()
    {
        // initialize product details 
        if (productData != null)
        {
            SpawnProductUI();
        }

        // update displayed quantity
        UpdateQuantityText();
    }

    // initializes product data and references the shop
    public void Initialize(ProductData product, UI_StockShop shopReference)
    {
        productData = product;
        ShopScript = shopReference;
        SpawnProductUI();
    }

    // sets ui up 
    private void SpawnProductUI()
    {
        productImage.sprite = productData.ProductImage;
        productTitle.text = productData.ProductName;
        productPrice.text = $"${productData.Price:F2}";
    }

    public void IncreaseQuantity()
    {
        quantity++;
        UpdateQuantityText();
    }

    public void DecreaseQuantity()
    {
        quantity = Mathf.Max(1, quantity - 1); 
        UpdateQuantityText();
    }

    private void UpdateQuantityText()
    {
        quantityText.text = quantity.ToString();
    }

    public void AddToBasket()
    {
        if (ShopScript != null)
        {
            for (int i = 0; i < quantity; i++)
            {
                ShopScript.SendToBasket(productData.ProductName);
            }
        }
        else
        {
            Debug.Log("no shop!");
        }
    }
}
