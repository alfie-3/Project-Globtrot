using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UI_Basket : MonoBehaviour
{

    [SerializeField] private UI_StockShop shopScript;

    //basket variables
    [SerializeField] private GameObject productBasketPrefab; 
    [SerializeField] private TMP_Text totalPriceTXT; 
    [SerializeField] private Transform basketParent; // makes sure products sent to basket appear correctly
    [SerializeField] private Transform basketStartPos; // makes sure basket appears in correct place
    [SerializeField] private int maxBasketSize = 10; // so the ui doesnt go off screen
    private List<GameObject> basket = new List<GameObject>(); 
    private float spacingY = 50.0f;
    public float SalePercentage;

    // adds to basket
    public void SendToBasket(string productName)
    {
        // check basket limit + if product exists
        if (!shopScript.productPrefabs.ContainsKey(productName)) return;

        ItemBase itemData = null;
        if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out itemData))

        // check if already present
        foreach (GameObject item in basket)
        {
            UI_BasketProduct UIproductScript = item.GetComponent<UI_BasketProduct>();
            if (UIproductScript != null && UIproductScript.GetProductName() == productName)
            {
                UIproductScript.IncreaseAmount();
                return; // return - to not add same item again
            }
        }

        // if item not present, spawn it into basket
        if (basket.Count < maxBasketSize)
        {
            Vector3 spawnPos = basketStartPos.position - new Vector3(0, spacingY * basket.Count, 0);
            GameObject UIProduct = Instantiate(productBasketPrefab, spawnPos, Quaternion.identity, basketParent);

            UI_BasketProduct UIProductScript = UIProduct.GetComponent<UI_BasketProduct>();
            
            if (itemData is ShopProduct_Item productItem)
            {
                UIProductScript.UpdateProduct(productItem, 1); // Send ShopProduct_Item
            }
            else if (itemData is PlacableFurniture_Item furnitureItem)
            {
                UIProductScript.UpdateProduct(furnitureItem, 1); // Send PlacableFurniture_Item
            }


            UIProductScript.SetShop(this, productName);
            basket.Add(UIProduct);
            UpdateBasket();
        }
        UpdateTotal();
    }

    // purchase + summon products
    public void Purchase()
    {
        foreach (GameObject uiProduct in basket.ToArray()) 
        {
            UI_BasketProduct UIproductScript = uiProduct.GetComponent<UI_BasketProduct>();
            if (UIproductScript != null)
            {
                int amountToSpawn = UIproductScript.GetAmount();
                string productName = UIproductScript.GetProductName();
                
                if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out ItemBase item))
                {
                    if (item is ShopProduct_Item productItem)
                    {
                        double totalPrice = productItem.GetCurrentPurchasePrice()* amountToSpawn;

                        // check if enough money
                        if (MoneyManager.Instance.CanAfford(totalPrice))
                        {
                            MoneyManager.Instance.SpendMoney(totalPrice);
                            
                            for (int i = 0; i < amountToSpawn; i++)
                            {
                                shopScript.SummonItem(productName);
                            }

                            UIproductScript.Trash();  // remove from basket after purchase
                        }
                        else
                        {
                            Debug.LogWarning($"Not enough money to buy {productName}!");
                        }
                    }
                    else if (item is PlacableFurniture_Item furnitureItem)
                    {
                        double totalPrice = furnitureItem.FurniturePrice * amountToSpawn;

                        // Check if enough money
                        if (MoneyManager.Instance.CanAfford(totalPrice))
                        {
                            MoneyManager.Instance.SpendMoney(totalPrice);
                            
                            for (int i = 0; i < amountToSpawn; i++)
                            {
                                shopScript.SummonItem(productName);  
                            }

                            UIproductScript.Trash();  
                        }
                        else
                        {
                            Debug.LogWarning($"Not enough money to buy {productName}!");
                        }
                    }
                }
            }
        }
        UpdateTotal();

    }


    public void RemoveItem(UI_BasketProduct product)
    {
        int removedIndex = basket.IndexOf(product.gameObject);

        basket.Remove(product.gameObject);

        Destroy(product.gameObject);

        UpdateBasket();
    }

    // update basket
    private void UpdateBasket()
    {
        for (int i = 0; i < basket.Count; i++)
        {
            Vector3 targetPos = basketStartPos.localPosition - new Vector3(0, spacingY * i, 0);
            if(basket[i]) StartCoroutine(BasketUP(basket[i].transform, targetPos));
            UpdateTotal();
        }
    }

    // animation for item leaving/joining basket
    private IEnumerator BasketUP(Transform obj, Vector3 targetPos)
    {
        float duration = 0.3f;
        float elapsedTime = 0;
        Vector3 startPos = obj.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            if(obj!= null) obj.localPosition = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

    }

    // update total price
    public void UpdateTotal()
    {
        double total = 0.0f;

        foreach (GameObject item in basket)
        {
            UI_BasketProduct UIproductScript = item.GetComponent<UI_BasketProduct>();
            if (UIproductScript != null)
            {
                string productName = UIproductScript.GetProductName();
                int amount = UIproductScript.GetAmount();

                if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out ItemBase itemData))
                {
                    if (itemData is ShopProduct_Item productItem)
                    {
                        total += productItem.GetCurrentPurchasePrice() * amount;
                    }
                }
            }
        }

        // update ui
        totalPriceTXT.text = $"Total: ${total:F2}";
    }
}
