using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHoldingManager : NetworkBehaviour
{
    public NetworkObject HeldObj { get; private set; }

    [SerializeField] public Material Material;

    [SerializeField] HoldingItemSocket ItemSocket;

    public float Rotation { get; private set; }

    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnRotate += PerformRotate;
        playerInputManager.OnPerformDrop += PerformDrop;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public void HoldItem(Pickup_Interactable obj)
    {
        if (HeldObj != null) { return; }

        if (obj == null)
        {
            return;
        }

        if (!obj.TryGetComponent(out NetworkObject nwObject)) return;

        HeldObj = nwObject;

        if (obj.TryGetComponent(out IOnHeld useableObject))
        {
            useableObject.OnHeld(this);
        }

        ItemSocket.BindObject_Rpc(HeldObj);
    }

    public void DropItem()
    {

        //To be added
    }

    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(NetworkObjectReference placedItem, string itemId, Vector3 location, Quaternion rotation)
    {
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log($"Placing furntiure item {itemId}");

        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab, location + placeableItem.FurniturePrefab.transform.position, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

        if (placedItem.TryGet(out NetworkObject networkObject))
        {
            networkObject.Despawn();
            RequestClearItem_Rpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void GiveCrate_RPC(string itemID)
    {
        PlacableFurniture_Item placeableItem = ItemDictionaryManager.RetrieveItem("Crate") is not PlacableFurniture_Item ? null : (PlacableFurniture_Item)ItemDictionaryManager.RetrieveItem("Crate");
        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.FurniturePrefab).GetComponent<NetworkObject>();
        instance.Spawn();

        instance.GetComponent<Pickup_Interactable>().OnInteract(GetComponent<PlayerInteractionManager>());
    }

    public void Update()
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IUpdate update))
        {
            update.OnUpdate(this);
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateItemPosition_RPC(Vector3 position, NetworkObjectReference item)
    {
        if (item.TryGet(out NetworkObject obj))
        {
            obj.transform.position = position;
        }
    }

    public void PerformPrimary(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (!HeldObj.TryGetComponent(out IUsePrimary useableObject)) return;

        useableObject.UsePrimary(this);
    }

    public void PerformSecondary(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (!HeldObj.TryGetComponent(out IUseSecondary useableObject)) return;

        useableObject.UseSecondary(this);
    }

    public void PerformDrop(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IOnDrop useableObject))
        {
            useableObject.OnDrop(this);
        }

        ItemSocket.ClearObjectBinding_Rpc();

        useableObject.OnDrop(this);
    }

    public void PerformRotate(float dir)
    {
        if (HeldObj == null) return;

        Rotation += dir * 22.5f;
    }

    [Rpc(SendTo.Everyone)]
    public void RequestClearItem_Rpc()
    {
        ClearItem();
    }

    public void ClearItem()
    {
        HeldObj = null;
        Rotation = 0;
    }
}
