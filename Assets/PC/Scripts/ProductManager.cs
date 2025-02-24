using UnityEngine;
using System.Collections.Generic;


//set up category differentiation for panels - currently they all go into the first category
//separate lists per category + add to scriptable obj what category they should go in
public class ProductManager : MonoBehaviour
{
    [SerializeField] private GameObject productUIPrefab;
    [SerializeField] private UI_StockShop ShopScript;
    [SerializeField] private List<Transform> panelParents = new List<Transform>();
    private List<Transform> basePanels = new List<Transform>(); 


    private List<ShopProduct_Item> allProducts = new List<ShopProduct_Item>();

    private void Start()
    {
        GetPanels();
        ItemDictionaryManager.RegisterItems();  
        RegisterProducts();
        InitializeProductUI();
    }

    private void RegisterProducts()
    {
        if (ShopScript == null) return;

        // load all shop products from ItemDictionaryManager
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
                displayScript.Initialize(allProducts[i]);
            }
            else
            {
                basePanels[i].gameObject.SetActive(false);
            }
        }
    }
    
    private void GetPanels()
    {
        foreach (Transform parent in panelParents)
        {
            foreach(Transform child in parent)
            {
                basePanels.Add(child);
            }
        }
        Debug.Log("Registered " + basePanels.Count + " panels");
    }
}