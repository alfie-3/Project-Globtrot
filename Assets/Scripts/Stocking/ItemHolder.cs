using System.Collections.Generic;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;
using Unity.VisualScripting;

public class ItemHolder : NetworkBehaviour
{
    public Action<string, string, int> OnStockUpdated = delegate { };
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [field: SerializeField] public ShopProduct_Item ProductItem { get; private set; }
    [field: SerializeField] public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool IsEmpty { get { return ItemQuantity.Value == 0; } }
    [field: SerializeField] public int maxItems { get; private set; }


    [Flags]
    public enum ContainerTypes {
        standard = 1,
        refrigirated = 2,
        frozen = 4,
        heated = 8,
        security = 16
    }


    [field: SerializeField] public ContainerTypes containerType { get; private set; }

    [Space(10)]
    [SerializeField] int initialQuanitity = 10;



    public void Awake()
    {
        ItemId.OnValueChanged += SetItem;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (ProductItem != null && IsServer)
        {
            int num = maxItems;
            AddItemServer_Rpc(ProductItem.ItemID, initialQuanitity);
            Debug.Log(num);
            maxItems = num;
            ItemQuantity.Value = initialQuanitity;
        }
    }


    [Rpc(SendTo.Server)]
    public void AddItemServer_Rpc(string itemId, int quantity = 1)
    {
        
        if (IsEmpty)
        {
            if (ItemDictionaryManager.RetrieveItem(itemId.ToString()) is not ShopProduct_Item || ((ItemDictionaryManager.RetrieveItem(itemId.ToString()) as ShopProduct_Item).ContanierCompatabilty & containerType) == 0) return;

            ItemId.Value = itemId;
        }
        quantity =  Math.Clamp(quantity, 0, maxItems - ItemQuantity.Value);
        if (quantity == 0) return;
        if (ItemId.Value.ToString() == itemId)
        {
            ItemQuantity.Value += quantity;
            OnStockUpdated.Invoke(itemId, itemId, ItemQuantity.Value);
        }

    }


    public bool RemoveItem(int quantity = 1)
    {
        if (ItemQuantity.Value < quantity) quantity = ItemQuantity.Value;
        if (quantity == 0) return false;
        if (IsServer)
        {
            ItemQuantity.Value -= quantity;
        }

        if (ItemQuantity.Value <= 0)
        {
            ClearItem();
        }
        else
        {
            OnStockUpdated.Invoke(ItemId.Value.ToString(), ItemId.Value.ToString(), ItemQuantity.Value);
        }

        return true;
    }

    public void ClearItem()
    {
        OnStockUpdated.Invoke(ItemId.Value.ToString(), string.Empty, 0);
        ItemId.Value = string.Empty;
    }

    public void SetItem(FixedString32Bytes _, FixedString32Bytes itemId)
    {
        if (itemId.Value.IsNullOrEmpty()) return;

        ShopProduct_Item productItem = ItemDictionaryManager.RetrieveItem(itemId.ToString()) is not ShopProduct_Item ? null : (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemId.ToString());
        ProductItem = productItem;
        maxItems = productItem.MaxInBox;
        OnStockUpdated.Invoke(string.Empty, itemId.ToString(), ItemQuantity.Value);
    }

    public static bool TransferItems(ItemHolder giver, ItemHolder reciver, int quantity = 1)
    {
        if (giver.IsEmpty || (!reciver.IsEmpty && giver.ItemId.Value != reciver.ItemId.Value))
            return false;

        string id = giver.ItemId.Value.ToString();

        if (((ItemDictionaryManager.RetrieveItem(id) as ShopProduct_Item).ContanierCompatabilty & reciver.containerType) == 0) return false;

        quantity = Math.Min(quantity, giver.ItemQuantity.Value);
        if (!reciver.IsEmpty)
            quantity = Math.Min(quantity, reciver.maxItems - reciver.ItemQuantity.Value);

        if (quantity <= 0) return false;

        
        giver.RemoveItem(quantity);
        reciver.AddItemServer_Rpc(id, quantity);

        return true;
    }



    public void SetMaxItems(int max) { maxItems = max; }
}
