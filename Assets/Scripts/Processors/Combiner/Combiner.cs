using DG.Tweening;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Combiner : NetworkBehaviour
{

    [SerializeField]
    CombinerRecipeList recipeList;
    [SerializeField] float proccesingTime;
    [SerializeField] float cogSpeedMult;
    [Space]
    [SerializeField]
    UI_CombinerScreen screen;
    [SerializeField]
    Transform itemOutput;
    [SerializeField]
    ItemSlot itemslot;
    [SerializeField]
    ItemSlot materialSlot;
    Animator anim;

    


    private void Awake()
    {
        itemslot.itemId.OnValueChanged += NewState;
        materialSlot.itemId.OnValueChanged += NewState;
        anim = GetComponent<Animator>();
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

    private bool makingItem;
    public void TryMakeItem()
    {
        if (makingItem) return;
        Stock_Item item = recipeList.GetItem(itemslot.itemId.Value.ToString(), materialSlot.itemId.Value.ToString());
        if (item == null) return;
        makingItem = true;
        if (IsServer)
        {
            itemslot.ClearItem();
            materialSlot.ClearItem();
        }
        float speedModifier = GlobalProcessorModifiers.CombinerSpeedMultiplier;
        DOTween.Sequence().Append(DOTween.To(() => anim.speed, x => anim.speed = x, cogSpeedMult * speedModifier, (proccesingTime/ speedModifier) *0.5f)).Append(DOTween.To(() => anim.speed, x => anim.speed = x, 1, (proccesingTime / speedModifier) *0.5f)).OnKill(() => 
        {
            if(IsServer)
                PlaceItem_Rpc(item.ItemID, itemOutput.position, itemOutput.rotation);
            makingItem = false;
        });//asd
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
