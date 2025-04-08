using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Fabricator : NetworkBehaviour
{
    [SerializeField]
    FabricatorRecipeList recipeList;
    [SerializeField]
    TextMeshPro indicator;
    [SerializeField]
    Transform itemOutput;

    FabricatorRecipeList.FabricatorRecipe currentrecipe;

    NetworkVariable<int> _recipeIndex = new();
    int recipeIndex { get { return _recipeIndex.Value; } set { _recipeIndex.Value = value < 0 ? recipeList.recipes.Count-1 : value >= recipeList.recipes.Count ? 0 : value; } }


    private void Awake() {
        _recipeIndex.OnValueChanged += indexChange;
        indexChange(1, 0);
    }

    public void NextItem() { recipeIndex++; }
    public void PreviousItem() { recipeIndex--; }

    void indexChange(int oldValue,int newValue){
        currentrecipe = recipeList.recipes[newValue];
        indicator.text = currentrecipe.name;
        Debug.Log(newValue);
    }


    public bool MakeItem(Stock_Item material) {
        Stock_Item item = recipeList.GetItem(material, currentrecipe);

        PlaceItem_Rpc(item.ItemID, itemOutput.position, itemOutput.rotation);
        return item != null;
    }



    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(string itemId, Vector3 location, Quaternion rotation) {
        Stock_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log($"Placing furntiure item {itemId}");

        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.Prefab, location, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

    }



    /*static Dictionary<string, FabOut> ham = new
    {
        {"name",StockItem[] items = {asd, asd, new StockItem(), }; }
    }*/
}
