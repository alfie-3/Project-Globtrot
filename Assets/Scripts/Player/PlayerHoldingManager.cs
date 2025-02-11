using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class PlayerHoldingManager : NetworkBehaviour
{
    [field: SerializeField] public ItemBase HeldItem { get; private set; }
    public Pickup_Interactable HeldObj { get; private set; }

    [SerializeField] public Material Material;
    public float Rotation { get; private set; }

    private NetworkObjectReference heldObjRef;
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

    public void HoldItem(ItemBase item, Pickup_Interactable obj)
    {
        if (HeldItem != null) { return; }

        if (item == null)
        {
            Debug.LogWarning("No referenced item - Did you forget to assign an item?");
            return;
        }

        if (obj is PickUp_Funiture) {
            GiveCrate_RPC(item.ItemID);
        } else {
            HeldObj = obj;
            heldObjRef = HeldObj.GetComponent<NetworkObject>();
        }
        HeldItem = item;
        HeldItem.OnHeld();
        
    }

    public void DropItem()
    {

        //To be added
    }

    [Rpc(SendTo.Server)]
    public void PlaceItem_Rpc(string itemId, Vector3 location, Quaternion rotation)
    {
        Debug.Log("Tammy");
        Placable_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not Placable_Item ? null : (Placable_Item)ItemDictionaryManager.RetrieveItem(itemId);
        Debug.Log("Nammy");
        Debug.Log(itemId);
        if (placeableItem == null) return;
        Debug.Log("Hammy");
        NetworkObject instance = Instantiate(placeableItem.PlaceablePrefab, location + placeableItem.PlaceablePrefab.transform.position, rotation).GetComponent<NetworkObject>();
        instance.Spawn();

        //HeldObj.GetComponent<NetworkObject>().Despawn();
        //HeldObj.RequestRemove_RPC();
    }
    [Rpc(SendTo.Server)]
    private void GiveCrate_RPC(string itemID) {
        Placable_Item placeableItem = ItemDictionaryManager.RetrieveItem("Crate") is not Placable_Item ? null : (Placable_Item)ItemDictionaryManager.RetrieveItem("Crate");
        if (placeableItem == null) return;
        NetworkObject instance = Instantiate(placeableItem.PlaceablePrefab).GetComponent<NetworkObject>();
        instance.Spawn();

        //HeldObj = instance.GetComponent<Pickup_Interactable>();
        //heldObjRef = instance.GetComponent<NetworkObject>();
        instance.GetComponent<Pickup_Interactable>().item = ItemDictionaryManager.RetrieveItem(itemID);
        instance.GetComponent<Pickup_Interactable>().OnInteract(GetComponent<PlayerInteractionManager>());
    }

    public void Update() {
        if (HeldItem == null) return;
        //HeldObj.transform.localPosition = transform.position + transform.forward;
        if(HeldObj.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            UpdateItemPosition_RPC(transform.position + (transform.Find("PlayerModelHolder").forward*0.5f), networkObject);
        
        HeldItem.OnUpdate(this);
    }

    [Rpc(SendTo.Server)]
    private void UpdateItemPosition_RPC(Vector3 position, NetworkObjectReference item) {
        if (item.TryGet(out NetworkObject obj)) {
            obj.transform.position = position;
        }
    }

    public void PerformPrimary()
    {
        if (HeldItem == null) return;

        HeldItem.OnPrimary(this);
    }

    public void PerformSecondary()
    {
        if (HeldItem == null) return;

        HeldItem.OnSecondary(this);
    }

    public void PerformDrop() 
    {
        if (HeldItem == null) return;
        Debug.Log("Drop");

        HeldObj.OnDrop(this);
    }

    public void PerformRotate(float dir)
    {
        if (HeldItem == null) return;

        Rotation += dir * 22.5f;
    }

    public void ClearItem()
    {
        HeldItem = null;
        Rotation = 0;
    }
}
