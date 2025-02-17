using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class StockBoxController : NetworkBehaviour, IUsePrimary
{
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public bool IsEmpty { get; private set; }
    public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);

    public void Awake() {
        IsEmpty = true;
    }


    public void UsePrimary(PlayerHoldingManager holdingManager) {
        Debug.Log("stockboxPri");
        Debug.Log(ItemId.Value.ToString());
        if (IsEmpty) return;


        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        if(Physics.Raycast(ray, out RaycastHit hit,5,LayerMask.GetMask("ItemShelf"))) 
        {
            if (hit.collider.TryGetComponent<StockShelfController>(out StockShelfController stockShelfController)) {
                stockShelfController.AddItem(ItemId.Value.ToString());
                RemoveItem(ItemId.Value.ToString());
            }
        }
    }
    public void AddItem(string itemId)
    {
        if (IsEmpty)
            SetItem(itemId);
        if (ItemId.Equals(itemId))
            ItemQuantity.Value++;

    }

    

    public void RemoveItem(string itemId)
    {
        if (itemId != ItemId.Value.ToString()) return;

        ItemQuantity.Value--;

        if (ItemQuantity.Value <= 0)
        {
            ClearItem();
        }
    }

    public void ClearItem()
    {
        ItemId.Value = string.Empty;
        IsEmpty = true;
    }

    public void SetItem(string itemId)
    {
        ItemId.Value = itemId;
        IsEmpty = false;
    }
}
