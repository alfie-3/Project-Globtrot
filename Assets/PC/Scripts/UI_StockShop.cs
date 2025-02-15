using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class UI_StockShop : MonoBehaviour
{
    [SerializeField] private GameObject productBasketPrefab; 
    [SerializeField] private Transform basketParent; // makes sure products sent to basket appear correctly
    [SerializeField] private Transform basketStartPos; // makes sure basket appears in correct place
    [SerializeField] private Transform spawnArea; // where purchased products will appear
    [SerializeField] private int maxBasketSize = 10; // so the ui doesnt go off screen
    [SerializeField] private List<GameObject> spawnedProducts = new List<GameObject>();
    private Dictionary<string, GameObject> productPrefabs = new Dictionary<string, GameObject>();
    private List<GameObject> basket = new List<GameObject>(); 
    private float spacingY = 100.0f;

    // registers given prefab into dictionary
    public void Register(string productName, GameObject productPrefab)
    {
        if (!productPrefabs.ContainsKey(productName))
        {
            productPrefabs.Add(productName, productPrefab);
        }
    }

    // adds to basket
    public void SendToBasket(string productName)
    {
        // check basket limit + if product exists
        if (basket.Count >= maxBasketSize || !productPrefabs.ContainsKey(productName)) return;

        // calculate spawn pos for new basket item
        Vector3 spawnPos = basketStartPos.position - new Vector3(0, spacingY * basket.Count, 0);
        GameObject uiProduct = Instantiate(productBasketPrefab, spawnPos, Quaternion.identity, basketParent);

        // update product UI and reference the shop
        UI_BasketProduct uiProductComponent = uiProduct.GetComponent<UI_BasketProduct>();
        uiProductComponent.UpdateProduct(productName, 1); 
        uiProductComponent.SetShop(this, productName);

        basket.Add(uiProduct);
        UpdateBasket();
    }

    // purchase + summon products
    public void Purchase()
    {
        foreach (GameObject uiProduct in basket.ToArray()) 
        {
            UI_BasketProduct productComponent = uiProduct.GetComponent<UI_BasketProduct>();
            if (productComponent != null)
            {
                int amountToSpawn = productComponent.GetAmount();
                string productName = productComponent.GetProductName();
                
                if (ItemDictionaryManager.ItemDict.TryGetValue(productName, out ItemBase item))
                {
                    if (item is ShopProduct_Item productItem)
                    {
                        float totalPrice = productItem.Price * amountToSpawn;

                        // check if enough money
                        if (MoneyManager.Instance.CanAfford(totalPrice))
                        {
                            MoneyManager.Instance.SpendMoney(totalPrice);

                            for (int i = 0; i < amountToSpawn; i++)
                            {
                                SummonProduct(productName);
                            }

                            productComponent.Trash();  // remove from basket after purchase
                        }
                        else
                        {
                            Debug.LogWarning($"Not enough money to buy {productName}!");
                        }
                    }
                }
            }
        }
    }


    // spawns product in spawn area
    private void SummonProduct(string productName)
    {
        if (!productPrefabs.ContainsKey(productName)) return;

        GameObject productPrefab = productPrefabs[productName];
        Vector3 spawnPos = spawnArea.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GameObject spawnedProduct = Instantiate(productPrefab, spawnPos, Quaternion.identity);
        spawnedProducts.Add(spawnedProduct);
    }

    public void RemoveItem(UI_BasketProduct product)
    {
        int removedIndex = basket.IndexOf(product.gameObject);

        basket.Remove(product.gameObject);

        Destroy(product.gameObject);

        StartCoroutine(BasketUP(removedIndex));
    }

    private IEnumerator BasketUP(int startIndex)
    {
        float animDuration = 0.1f; 
        float elapsedTime;

        for (int i = startIndex; i < basket.Count; i++)
        {
            Vector3 startPos = basket[i].transform.localPosition;
            Vector3 targetPos = basketStartPos.localPosition - new Vector3(0, spacingY * i, 0);

            elapsedTime = 0;
            
            while (elapsedTime < animDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / animDuration);

                basket[i].transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
            }

            basket[i].transform.localPosition = targetPos;
        }
    }
    private void UpdateBasket()
    {
        for (int i = 0; i < basket.Count; i++)
        {
            Vector3 targetPos = basketStartPos.localPosition - new Vector3(0, spacingY * i, 0);
            basket[i].transform.localPosition = targetPos;
        }
    }




}
