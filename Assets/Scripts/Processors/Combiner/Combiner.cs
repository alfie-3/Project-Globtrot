using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Combiner : NetworkBehaviour
{

    [SerializeField]
    CombinerRecipeList recipeList;
    [SerializeField]
    TextMeshPro indicator;
    [SerializeField]
    Transform itemOutput;
    [SerializeField]
    ItemSlot itemslot;
    [SerializeField]
    ItemSlot materialSlot;










    public void MakeItem()
    {
        Stock_Item ham = itemslot.Item.GetComponent<StockItem>().Item;
        Stock_Item Gama = materialSlot.Item.GetComponent<StockItem>().Item;
        Stock_Item item = recipeList.GetItem(ham,Gama );

        PlaceItem_Rpc(item.ItemID, itemOutput.position, itemOutput.rotation);
        //return item != null;
    }


    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(string itemId, Vector3 location, Quaternion rotation)
    {
        Stock_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log($"Placing furntiure item {itemId}");

        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.Prefab, location, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

    }
}
