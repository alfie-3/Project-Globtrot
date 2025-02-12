using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    private int basketCounter = 0;
    private float spacingY = 75f;

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
        if (basketCounter >= maxBasketSize || !productPrefabs.ContainsKey(productName)) return;

        // calculate spawn pos for new basket item
        Vector3 spawnPos = basketStartPos.position - new Vector3(0, spacingY * basketCounter, 0);
        GameObject uiProduct = Instantiate(productBasketPrefab, spawnPos, Quaternion.identity, basketParent);

        // update product UI and reference the shop
        UI_BasketProduct uiProductComponent = uiProduct.GetComponent<UI_BasketProduct>();
        uiProductComponent.UpdateProduct(productName, 1); 
        uiProductComponent.SetShop(this, productName);

        basket.Add(uiProduct);
        basketCounter++;
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
                for (int i = 0; i < amountToSpawn; i++)
                {
                    SummonProduct(productComponent.GetProductName());
                }
                productComponent.Trash();
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

    public void RemoveFromBasket(UI_BasketProduct product)
    {
        basket.Remove(product.gameObject);
        basketCounter = Mathf.Max(0, basketCounter - 1); // Prevents negative counter
    }
}
