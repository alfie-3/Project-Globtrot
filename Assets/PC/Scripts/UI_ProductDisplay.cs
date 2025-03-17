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
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TMP_Text unlockPriceText;

    private UI_Basket basketScript;
    private ShopProduct_Item productData;
    private PlacableFurniture_Item furnitureData;
    private int quantity = 1;
    private bool isUnlocked = false;

    public void Initialize(ShopProduct_Item product, UI_Basket basket)
    {
        basketScript = basket;
        productData = product;
        isUnlocked = PlayerPrefs.GetInt($"Unlocked_{product.ItemID}", 0) == 1;

        productImage.sprite = product.ItemIcon;
        productTitle.text = product.ItemName;
        productPrice.text = $"${product.GetCurrentPurchasePrice():F2}";

        SetupUnlockUI(product);
    }

    public void InitializeFurniture(PlacableFurniture_Item furniture, UI_Basket basket)
    {
        basketScript = basket;
        furnitureData = furniture;
        isUnlocked = PlayerPrefs.GetInt($"Unlocked_{furniture.ItemID}", 0) == 1;

        productImage.sprite = furniture.ItemIcon;
        productTitle.text = furniture.ItemName;
        productPrice.text = $"${furniture.GetCurrentPurchasePrice():F2}";

        SetupUnlockUI(furniture);
    }

    private void SetupUnlockUI(ShopProduct_Item item)
    {
        if (!item.Unlockable)
        {
            isUnlocked = true; 
            PlayerPrefs.SetInt($"Unlocked_{item.ItemID}", 1);
        }

        if (item.Unlockable && !isUnlocked)
        {
            lockPanel.SetActive(true);
            unlockButton.gameObject.SetActive(true);
            unlockPriceText.gameObject.SetActive(true);
            unlockPriceText.text = $"Unlock: ${item.UnlockPrice:F2}";
            unlockButton.onClick.AddListener(() => UnlockProduct(item));
        }
        else
        {
            lockPanel.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            unlockPriceText.gameObject.SetActive(false);
        }
    }

    private void SetupUnlockUI(PlacableFurniture_Item furniture)
    {
        if (!furniture.Unlockable)
        {
            isUnlocked = true;  
            PlayerPrefs.SetInt($"Unlocked_{furniture.ItemID}", 1);
        }

        if (furniture.Unlockable && !isUnlocked)
        {
            lockPanel.SetActive(true);
            unlockButton.gameObject.SetActive(true);
            unlockPriceText.gameObject.SetActive(true);

            unlockPriceText.text = $"Unlock: ${furniture.UnlockPrice:F2}";
            unlockButton.onClick.AddListener(() => UnlockProduct(furniture));
        }
        else
        {
            lockPanel.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            unlockPriceText.gameObject.SetActive(false);
        }
    }


    private void UnlockProduct(ShopProduct_Item item)
    {
        if (MoneyManager.Instance.CanAfford(item.UnlockPrice))
        {
            MoneyManager.Instance.SpendMoney(item.UnlockPrice);
            isUnlocked = true;
            PlayerPrefs.SetInt($"Unlocked_{item.ItemID}", 1);
            lockPanel.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            unlockPriceText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Not enough money to unlock!");
        }
    }    
    private void UnlockProduct(PlacableFurniture_Item furniture)
    {
        if (MoneyManager.Instance.CanAfford(furniture.UnlockPrice))
        {
            MoneyManager.Instance.SpendMoney(furniture.UnlockPrice);
            isUnlocked = true;
            PlayerPrefs.SetInt($"Unlocked_{furniture.ItemID}", 1);
            lockPanel.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            unlockPriceText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Not enough money to unlock!");
        }
    }

    public void AddToBasket()
    {
        if (isUnlocked)
        {
            if (basketScript != null)
            {
                if (productData != null)  
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        basketScript.SendToBasket(productData.ItemID); 
                    } 
                }
                else if (furnitureData != null)  
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        basketScript.SendToBasket(furnitureData.ItemID);  
                    }
                }
                else
                {
                    Debug.Log("Invalid item type!");
                }
            }
            else
            {
                Debug.Log("No shop!");
            }
        }
        else
        {
            Debug.Log("Product is locked!");
        }
    }

}
