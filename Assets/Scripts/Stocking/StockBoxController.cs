using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class StockBoxController : NetworkBehaviour, IUsePrimary
{
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [field: SerializeField]  public ShopProduct_Item ProductItem { get; private set; }

    public bool IsEmpty { get { return ItemId.Value.IsEmpty; }}

    [field: SerializeField] public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
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
            AddItemServer_Rpc(ProductItem.ItemID, initialQuanitity);
        }
    }

    public void UsePrimary(PlayerHoldingManager holdingManager) {
        //Debug.Log("stockboxPri");
        //Debug.Log(ItemId.Value.ToString());
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

    [Rpc(SendTo.Server)]
    public void AddItemServer_Rpc(string itemId, int quanitity = 1)
    {
        if (IsEmpty)
        {
            if (ItemDictionaryManager.RetrieveItem(ItemId.Value.ToString()) is ShopProduct_Item) return;
            ItemId.Value = itemId;
            ItemQuantity.Value = quanitity;
        }
        if (ItemId.Equals(itemId))
            ItemQuantity.Value += quanitity;

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
    }

    public void SetItem(FixedString32Bytes _, FixedString32Bytes itemId)
    {
        if (itemId.Value.IsNullOrEmpty()) return;

        ShopProduct_Item productItem = ItemDictionaryManager.RetrieveItem(itemId.ToString()) is not ShopProduct_Item ? null : (ShopProduct_Item)ItemDictionaryManager.RetrieveItem(itemId.ToString());
        ProductItem = productItem;
    }
}
