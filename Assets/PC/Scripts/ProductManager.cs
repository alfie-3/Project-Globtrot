using UnityEngine;
using System.Collections.Generic;

// manages products + spawns their displays in the shop ui
public class ProductManager : MonoBehaviour
{
    public ProductData[] AllProducts; 
    [SerializeField] private GameObject productUIPrefab;
    [SerializeField] private List<Transform> basePanels = new List<Transform>(); //panels in scene - help product ui display properly
    public UI_StockShop ShopScript;


    private void Start()
    {
        // register products with the shop
        RegisterProducts();
        
        // spawn ui for the products
        InitializeProductUI();
    }

    // registers each product's prefab with the shop for spawning when purchased
    private void RegisterProducts()
    {
        if (ShopScript == null) return;

        foreach (ProductData product in AllProducts)
        {
            if (product.Prefab != null)
            {
                ShopScript.Register(product.ProductName, product.Prefab);
            }
        }
    }

    // instantiates ui for each product - checking if  there is an available location
    private void InitializeProductUI()
    {
        for (int i = 0; i < basePanels.Count; i++)
        {
            if (i < AllProducts.Length)
            {
                GameObject productUIObj = Instantiate(productUIPrefab, basePanels[i].position, Quaternion.identity, basePanels[i]);
                
                // initialise product data with ui script
                UI_ProductDisplay displayScript = productUIObj.GetComponent<UI_ProductDisplay>();
                displayScript.Initialize(AllProducts[i], ShopScript);
            }
            else
            {
                // disable unused panels if there are more panels than products
                basePanels[i].gameObject.SetActive(false);
            }
        }
    }
}
