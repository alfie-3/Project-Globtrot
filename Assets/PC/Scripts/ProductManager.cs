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

    private List<ShopProduct_Item> allProducts = new List<ShopProduct_Item>();

    private void Start()
    {
        SetUpCategories();
        ItemDictionaryManager.RegisterItems();
        RegisterProducts();
        InitializeProductUI();
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

    private void RegisterProducts()
    {
        if (shopScript == null) return;

        foreach (var item in ItemDictionaryManager.ItemDict.Values)
        {
            if (item is ShopProduct_Item productItem)
            {
                allProducts.Add(productItem);
                shopScript.Register(productItem.ItemID, productItem.Prefab);
            }
        }
    }

    private void InitializeProductUI()
    {
        
        Dictionary<ProductCategory, int> c_productCount = new Dictionary<ProductCategory, int>();

        foreach (var product in allProducts)
        {
            // check if category has panels
            if (c_productPanels.TryGetValue(product.Category, out List<Transform> productPanels))
            {
                // check if a category already has any products - if not set count to 0
                int count = c_productCount.ContainsKey(product.Category) ? c_productCount[product.Category] : 0;

                //if available panel
                if (count < productPanels.Count) 
                {
                    GameObject productUIObj = Instantiate(productUIPrefab, productPanels[count]);
                    productUIObj.transform.position = productPanels[count].position; 

                    UI_ProductDisplay displayScript = productUIObj.GetComponent<UI_ProductDisplay>();
                    
                    displayScript.Initialize(product, basketScript);

                    // update product count for category
                    c_productCount[product.Category] = count + 1;
                }
            }
        }

        // after all needed panels used disable others
        foreach (var kvp in c_productPanels)
        {
            ProductCategory category = kvp.Key;
            List<Transform> panels = kvp.Value;

            int usedCount = c_productCount.ContainsKey(category) ? c_productCount[category] : 0;

            for (int i = usedCount; i < panels.Count; i++)
            {
                panels[i].gameObject.SetActive(false); 
            }
        }
    }

}

// example categories
public enum ProductCategory
{
    Drinks,
    Food
}