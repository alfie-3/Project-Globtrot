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
}
// example categories
public enum ProductCategory
{
    Drinks,
    Food,
    Furniture
}