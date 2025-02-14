using UnityEngine;
using System.Collections.Generic;

public class ProductManager : MonoBehaviour
{
    [SerializeField] private GameObject productUIPrefab;
    [SerializeField] private List<Transform> basePanels = new List<Transform>(); 
    public UI_StockShop ShopScript;

    private List<ShopProduct_Item> allProducts = new List<ShopProduct_Item>();

    private void Start()
    {
        ItemDictionaryManager.RegisterItems();  
        RegisterProducts();
        InitializeProductUI();
    }

    private void RegisterProducts()
    {
        if (ShopScript == null) return;

        // Load all shop products from ItemDictionaryManager
        foreach (var item in ItemDictionaryManager.ItemDict.Values)
        {
            if (item is ShopProduct_Item productItem)
            {
                allProducts.Add(productItem);
                ShopScript.Register(productItem.ItemID, productItem.Prefab);

            }
        }
    }

    private void InitializeProductUI()
    {
        for (int i = 0; i < basePanels.Count; i++)
        {
            if (i < allProducts.Count)
            {
                GameObject productUIObj = Instantiate(productUIPrefab, basePanels[i].position, Quaternion.identity, basePanels[i]);
                UI_ProductDisplay displayScript = productUIObj.GetComponent<UI_ProductDisplay>();
                displayScript.Initialize(allProducts[i], ShopScript);
            }
            else
            {
                basePanels[i].gameObject.SetActive(false);
            }
        }
    }
}
