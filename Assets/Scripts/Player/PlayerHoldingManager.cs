using Unity.Netcode;
using UnityEngine;

public class PlayerHoldingManager : NetworkBehaviour
{
    [field: SerializeField] public ItemBase HeldItem { get; private set; }
    [SerializeField] public Material Material;
    public float Rotation { get; private set; }

    public PlayerCameraManager CameraManager { get; private set; }

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnRotate += PerformRotate;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public void HoldItem(ItemBase item)
    {
        if (HeldItem != null) { return; }

        if (item == null)
        {
            Debug.LogWarning("No referenced item - Did you forget to assign an item?");
            return;
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

        Placable_Item placeableItem = ItemDictionaryManager.RetrieveItem(itemId) is not Placable_Item ? null : (Placable_Item)ItemDictionaryManager.RetrieveItem(itemId);
        if (placeableItem == null) return;

        NetworkObject instance = Instantiate(placeableItem.PlaceablePrefab, location + placeableItem.PlaceablePrefab.transform.position, rotation).GetComponent<NetworkObject>();
        instance.Spawn();
    }

    public void Update() {
        if (HeldItem == null) return;
        HeldItem.OnUpdate(this);
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

    public void PerformRotate(float dir)
    {
        if (HeldItem == null) return;

        Rotation += dir * 8;
    }

    public void ClearItem()
    {
        HeldItem = null;
        Rotation = 0;
    }
}
