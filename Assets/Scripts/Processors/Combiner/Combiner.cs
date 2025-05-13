using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Combiner : NetworkBehaviour
{

    [SerializeField]
    CombinerRecipeList recipeList;
    [SerializeField]
    UI_CombinerScreen screen;
    [SerializeField]
    Transform itemOutput;
    [SerializeField]
    ItemSlot itemslot;
    [SerializeField]
    ItemSlot materialSlot;

    Stock_Item Item;
    Stock_Item Material;



    private void Awake()
    {
        itemslot.OnItemAdded += (value) => { Item = value; screen.newState(Item, Material); };
        itemslot.OnItemRemoved += () => { Item = null; screen.newState(Item, Material); };
        materialSlot.OnItemAdded += (value) => { Material = value; screen.newState(Item, Material); };
        materialSlot.OnItemRemoved += () => { Material = null; screen.newState(Item, Material); };
    }

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
