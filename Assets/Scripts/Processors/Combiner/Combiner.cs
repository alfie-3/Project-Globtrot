using TMPro;
using Unity.Collections;
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

    //NetworkVariable<FixedString32Bytes> itemslot.itemId = new NetworkVariable<FixedString32Bytes>();
    //NetworkVariable<FixedString32Bytes> materialSlot.itemId = new NetworkVariable<FixedString32Bytes>();

    //Stock_Item Item;
    //Stock_Item Material;



    private void Awake()
    {
        itemslot.itemId.OnValueChanged += NewState;
        materialSlot.itemId.OnValueChanged += NewState;
        //itemslot.itemId.OnValueChanged += NewState;
        //materialSlot.itemId.OnValueChanged += NewState;
    }


    
    protected void NewState(FixedString32Bytes prevoisValue, FixedString32Bytes newValue)
    {
        string itemID = itemslot.itemId.Value.ToString();
        string materialID = materialSlot.itemId.Value.ToString();

        if (!string.IsNullOrEmpty(itemID) && !string.IsNullOrEmpty(materialID))
        {
            screen.EndScreen(recipeList.GetItem(itemID, materialID));
        }
        Stock_Item Item = string.IsNullOrEmpty(itemID) ? null : ItemDictionaryManager.RetrieveItem(itemID) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(itemID);
        Stock_Item Material = string.IsNullOrEmpty(materialID) ? null : ItemDictionaryManager.RetrieveItem(materialID) is not Stock_Item ? null : (Stock_Item)ItemDictionaryManager.RetrieveItem(materialID);
        screen.newState(Item, Material);
    }
    public void MakeItem()
    {
        Stock_Item item = recipeList.GetItem(itemslot.itemId.Value.ToString(), materialSlot.itemId.Value.ToString());
        if (item == null) return;

        PlaceItem_Rpc(item.ItemID, itemOutput.position, itemOutput.rotation);
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
