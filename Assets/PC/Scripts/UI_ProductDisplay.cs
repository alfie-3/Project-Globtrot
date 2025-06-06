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
    private Stock_Item productData;
    private PlacableFurniture_Item furnitureData;
    private int quantity = 1;
    private bool isUnlocked = false;

    // set up variables
    public void Initialize(Stock_Item productScript, UI_Basket basketScript)
    {
        this.basketScript = basketScript;
        productData = productScript;
        isUnlocked = PlayerPrefs.GetInt($"Unlocked_{productScript.ItemID}", 0) == 1;

        productImage.sprite = productScript.ItemIcon;
        productTitle.text = productScript.ItemName;
        UpdateQuantity();

        SetupUnlockUI(productScript);
    }

    // check and set up locking mechanic
    private void SetupUnlockUI(Stock_Item item)
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


    // function for unlocking button
    private void UnlockProduct(Stock_Item item)
    {
        if (MoneyManager.Instance.CanAfford(item.UnlockPrice))
        {
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
        if (MoneyManager.Instance.CanAfford(furniture.FurniturePrice))
        {
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
            }
        }
        quantity = 1;
    }

    private void IncreaseQuantity()
    {
        quantity++;
        UpdateQuantity();
    }

    private void DecreaseQuantity()
    {
        if (quantity > 1)
        {
            quantity--;
            UpdateQuantity();
        }
    }
    private void UpdateQuantity()
    {
        quantityText.text = quantity.ToString();
    }
}
