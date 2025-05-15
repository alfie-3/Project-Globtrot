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
        itemslot.OnItemAdded += (value) => { Item = value; NewState(); };
        itemslot.OnItemRemoved += () => { Item = null; NewState(); };
        materialSlot.OnItemAdded += (value) => { Material = value; NewState(); };
        materialSlot.OnItemRemoved += () => { Material = null; NewState(); };
    }

    protected void NewState()
    {
        if (Item != null && Material != null)
        {
            screen.EndScreen(recipeList.GetItem(Item, Material));
        }
        screen.newState(Item, Material);
    }
    public void MakeItem()
    {
        PlaceItem_Rpc(recipeList.GetItem(Item, Material).ItemID, itemOutput.position, itemOutput.rotation);
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
