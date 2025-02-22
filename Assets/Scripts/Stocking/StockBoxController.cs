using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class StockBoxController : NetworkBehaviour, IUsePrimary, IUseSecondary
{
    public NetworkVariable<FixedString32Bytes> ItemId { get; private set; } = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [field: SerializeField]  public ShopProduct_Item ProductItem { get; private set; }

    public bool IsEmpty { get { return ItemId.Value.IsEmpty; }}

    [field: SerializeField] public NetworkVariable<int> ItemQuantity { get; private set; } = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] int initialQuanitity = 10;


    public void Awake()
    {
        ItemId.OnValueChanged += SetItem;
        //ItemQuantity.OnValueChanged += (() => text.text = $"{ItemId.Value.ToString()} \n {ItemQuantity.Value.ToString()}");
        
        ItemQuantity.OnValueChanged += (previousValue, newValue) => text.text = $"{ItemId.Value}\n{newValue}";

    }
    void UpdateText(int oldI, int newI)
    {
        text.text = $"{ItemId.Value}\n{newI}";
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
        Debug.Log("stockboxPri");
        Debug.Log(ItemId.Value.ToString());
        if (IsEmpty) return;


        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        if(Physics.Raycast(ray, out RaycastHit hit,5,LayerMask.GetMask("ItemShelf"))) 
        {
            if (hit.collider.TryGetComponent<StockShelfController>(out StockShelfController stockShelfController)) {
                stockShelfController.AddItemServer_Rpc(ItemId.Value.ToString());
                RemoveItem(ItemId.Value.ToString());
            }
        }
    }

    public void UseSecondary(PlayerHoldingManager holdingManager)
    {
        Debug.Log("stockboxPri");
        Debug.Log(ItemId.Value.ToString());
        if (IsEmpty) return;


        Ray ray = new(holdingManager.CameraManager.CamTransform.position, holdingManager.CameraManager.CamTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5);
        if (Physics.Raycast(ray, out RaycastHit hit, 5, LayerMask.GetMask("ItemShelf")))
        {
            if (hit.collider.TryGetComponent<StockShelfController>(out StockShelfController stockShelfController))
            {
                FixedString32Bytes shelfItems = stockShelfController.ItemId.Value;
                if (!IsEmpty && shelfItems != ItemId.Value) return;
                if(!stockShelfController.RemoveItem()) return;
                AddItemServer_Rpc(shelfItems.ToString());
            }
        }
    }



    [Rpc(SendTo.Server)]
    public void AddItemServer_Rpc(string itemId, int quanitity = 1)
    {
        Debug.Log("addeing item");
        if (IsEmpty)
        {
            if (ItemDictionaryManager.RetrieveItem(itemId.ToString()) is not ShopProduct_Item) return;
            ItemId.Value = itemId;
        }
        if (ItemId.Value.ToString() == itemId)
        {
            ItemQuantity.Value += quanitity;
        }

    }


    public void RemoveItem(string itemId)
    {
        if (itemId != ItemId.Value.ToString()) return;

        if (IsServer)
        {
            ItemQuantity.Value--;
        }

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
