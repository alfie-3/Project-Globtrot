using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "new CombinerRecipeList", menuName = "Lists/CombinerRecipeList")]
public class CombinerRecipeList : ScriptableObject
{
    [SerializeField]
    Stock_Item lightMaterial;
    [SerializeField]
    Stock_Item mediumMaterial;
    [SerializeField]
    Stock_Item heavyMaterial;

    public Stock_Item GetItem(Stock_Item item, Stock_Item material)
    {
        CombinerRecipe recipe = recipes.Where(x => x.item == item).First();
        if (material == lightMaterial) return recipe.lightVarient;
        if (material == mediumMaterial) return recipe.mediumVarient;
        if (material == heavyMaterial) return recipe.heavyVarient;
        return null;
    }
    public Stock_Item GetItem(string itemId, string materialId)
    {
        Stock_Item item = string.IsNullOrEmpty(itemId) ? null : ItemDictionaryManager.RetrieveItem(itemId) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Stock_Item material = string.IsNullOrEmpty(materialId) ? null : ItemDictionaryManager.RetrieveItem(materialId) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(materialId);
        if (item == null || material == null) return null;
        CombinerRecipe recipe = recipes.Where(x => x.item == item).First();
        if (material == lightMaterial) return recipe.lightVarient;
        if (material == mediumMaterial) return recipe.mediumVarient;
        if (material == heavyMaterial) return recipe.heavyVarient;
        return null;
    }
    //public
    [System.Serializable]
    public struct CombinerRecipe
    {
        public string name;
        public Stock_Item item;
        public Stock_Item lightVarient;
        public Stock_Item mediumVarient;
        public Stock_Item heavyVarient;
    }
    [SerializeField]
    public List<CombinerRecipe> recipes;
}
