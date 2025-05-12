using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "new FabricatorRecipeList", menuName = "Lists/FabricatorRecipeList")]
public class FabricatorRecipeList : ScriptableObject
{

    [SerializeField]
    Stock_Item lightMaterial;
    [SerializeField]
    Stock_Item mediumMaterial;
    [SerializeField]
    Stock_Item heavyMaterial;

    public Stock_Item GetItem(Stock_Item material,FabricatorRecipe recipe) {
        if(material == lightMaterial) return recipe.lightVarient;
        if(material == mediumMaterial) return recipe.mediumVarient;
        if (material == heavyMaterial) return recipe.heavyVarient;
        return null;
    }

    //public
    [System.Serializable]
    public struct FabricatorRecipe {
        public string name;
        public Stock_Item lightVarient;
        public Stock_Item mediumVarient;
        public Stock_Item heavyVarient;
    }
    [SerializeField]
    public List<FabricatorRecipe> recipes;
}
