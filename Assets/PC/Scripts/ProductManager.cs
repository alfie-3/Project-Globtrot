using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class ProductManager : MonoBehaviour
{
    [SerializeField] private GameObject productUIPrefab;
    [SerializeField] private UI_StockShop shopScript;
    [SerializeField] private UI_Basket basketScript;
    [SerializeField] private List<Transform> panelParents = new List<Transform>();

    // c_ = category
    [SerializeField] private List<TextMeshProUGUI> c_titles = new List<TextMeshProUGUI>(); 
    [SerializeField] private List<TextMeshProUGUI> c_buttonTexts = new List<TextMeshProUGUI>(); 
    private Dictionary<ProductCategory, List<Transform>> c_productPanels = new Dictionary<ProductCategory, List<Transform>>();

    private List<Stock_Item> allProducts = new List<Stock_Item>();
    private List<PlacableFurniture_Item> allFurniture = new List<PlacableFurniture_Item>();


    private void Start()
    {
        SetUpCategories();
        ItemDictionaryManager.RegisterItems();
        RegisterProducts();
        InitializeAllProducts();
    }

    private void SetUpCategories()
    {
        ProductCategory[] categories = (ProductCategory[])System.Enum.GetValues(typeof(ProductCategory));

        // set panels + categorise them
        for (int i = 0; i < panelParents.Count && i < categories.Length; i++)
        {
            List<Transform> productPanels = new List<Transform>();
        
            foreach (Transform panel in panelParents[i])
            {
                productPanels.Add(panel);
            }

            c_productPanels[categories[i]] = productPanels;
        }

        //assign title
        for (int i = 0; i < c_titles.Count && i < categories.Length; i++)
        {
            c_titles[i].text = categories[i].ToString(); 
        }

        //assign button text
        for(int i = 0; i < c_buttonTexts.Count && i < categories.Length; i++) 
        {
            c_buttonTexts[i].text = categories[i].ToString();
        }
    }

    // get products + put in correct lists
    private void RegisterProducts()
    {
        if (shopScript == null) return;

        foreach (var item in ItemDictionaryManager.ItemDict.Values)
        {
            if (item is Stock_Item productItem )
            {
                allProducts.Add(productItem);
                shopScript.Register(productItem.ItemID, productItem.Prefab);
            }
            else if (item is PlacableFurniture_Item furnitureItem)
            {
                if(item.ItemID!= "crate")
                {
                    allFurniture.Add(furnitureItem);
                    shopScript.Register(furnitureItem.ItemID, furnitureItem.FurniturePrefab);
                }

            }
        }
    }

    //spawn in food related products
    private void InitializeProductUI(List<Stock_Item> products, ProductCategory category)
    {
        if (!c_productPanels.TryGetValue(category, out List<Transform> productPanels))
            return;

        int count = 0;
        
        foreach (var product in products)
        {
            if (count < productPanels.Count)
            {
                GameObject productUIObj = Instantiate(productUIPrefab, productPanels[count]);
                productUIObj.transform.position = productPanels[count].position; 

                UI_ProductDisplay displayScript = productUIObj.GetComponent<UI_ProductDisplay>();
                        
                displayScript.Initialize(product, basketScript);

                count++;
            }
        }

        DisableUnusedPanels(productPanels, count);
    }

    // spawn in furniture products
    private void InitializeProductUI(List<PlacableFurniture_Item> furnitureItems, ProductCategory category)
    {
        if (!c_productPanels.TryGetValue(category, out List<Transform> furniturePanels))
            return;

        int count = 0;

        foreach (var furniture in furnitureItems)
        {
            if (count < furniturePanels.Count)
            {
                GameObject productUIObj = Instantiate(productUIPrefab, furniturePanels[count]);
                productUIObj.transform.position = furniturePanels[count].position;

                UI_ProductDisplay displayScript = productUIObj.GetComponent<UI_ProductDisplay>();
                displayScript.InitializeFurniture(furniture, basketScript);

                count++;
            }
        }

        DisableUnusedPanels(furniturePanels, count);
    }

    // if theres extra unused panels for spawning products - get rid
    private void DisableUnusedPanels(List<Transform> panels, int usedCount)
    {
        for (int i = usedCount; i < panels.Count; i++)
        {
            panels[i].gameObject.SetActive(false);
        }
    }

    // goes through each category of items and spawns them in
    private void InitializeAllProducts()
    {
        //var foodProducts = allProducts.FindAll(p => p.Category == ProductCategory.Food);
        //InitializeProductUI(foodProducts, ProductCategory.Food);

        //var drinkProducts = allProducts.FindAll(p => p.Category == ProductCategory.Drinks);
        //InitializeProductUI(drinkProducts, ProductCategory.Drinks);

        InitializeProductUI(allFurniture, ProductCategory.Furniture);
    }
}
// example categories
public enum ProductCategory
{
    Drinks,
    Food,
    Furniture
}