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
