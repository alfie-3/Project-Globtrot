using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class StockBoxController : MonoBehaviour
{
    public NetworkVariable<FixedString64Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool IsEmpty { get; private set; }
    public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public void AddItem(string itemId)
    {
        if (IsEmpty)
            SetItem(itemId);

        ItemQuantity.Value++;
    }

    public void RemoveItem(string itemId)
    {
        if (itemId != ItemId.Value) return;

        ItemQuantity.Value--;

        if (ItemQuantity.Value <= 0)
        {
            ClearItem();
        }
    }

    public void ClearItem()
    {
        ItemId.Value = null;
        IsEmpty = true;
    }

    public void SetItem(string itemId)
    {
        ItemId.Value = itemId;
        IsEmpty = false;
    }
}
