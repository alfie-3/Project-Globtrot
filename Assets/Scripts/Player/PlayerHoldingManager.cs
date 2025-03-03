using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Windows;

public class PlayerHoldingManager : NetworkBehaviour
{
    [field: SerializeField] public NetworkObject HeldObj { get; private set; }

    [SerializeField] public Material Material;

    [SerializeField] HoldingItemSocket ItemSocket;

    //Dropping
    [Space]
    [Header("Dropping")]
    [SerializeField] float dropDistance = 2.5f;
    [SerializeField] float dropHeight = 0.5f;
    [SerializeField] LayerMask dropObjectLayerMask;

    [Header("Throwing")]
    [SerializeField] float initailThrowForce = 8f;
    [SerializeField] float throwForceGrowthRate = 8f;

    [HideInInspector]
    public bool SnappingEnabled;// { get; private set; }
    
    public PlayerCameraManager CameraManager { get; private set; }
    private bool throwing;

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnScroll += PerformScroll;
        playerInputManager.OnPerformDrop += PerformDrop;

        playerInputManager.OnPerformCtrl += PerformCtrl;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public void HoldItem(Pickup_Interactable obj)
    {
        if (HeldObj != null)
        {
            ItemSocket.ClearBoundObject();
            return;
        }

        if (obj == null) return;
        if (obj.PickedUp.Value == true) return;
        if (!obj.TryGetComponent(out NetworkObject nwObject)) return;
        HeldObj = nwObject;

        if (obj.TryGetComponent(out IOnHeld useableObject))
        {
            useableObject.OnHeld(this);
        }

        if (obj.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
        {
            rbNWT.WakeUpNearbyObjects();
        }

        ItemSocket.BindObject_Rpc(HeldObj);
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
    public void ThrowObject_RPC(NetworkObjectReference item)
    {
        if (item.TryGet(out NetworkObject networkObject))
        {
            networkObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }

    public void Update()
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IUpdate update))
        {
            update.OnUpdate(this);
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
        if (context.performed) {

            if (context.interaction is HoldInteraction) {
                if (!HeldObj.TryGetComponent(out IOnDrop onDrop)) return;
                throwing = true;
            }
            if (context.interaction is PressInteraction) {
                useableObject.UseSecondary(this);
            }
        } else {
            if (throwing)
            {
                throwing = false;
                if (!HeldObj.TryGetComponent(out IOnDrop onDrop)) return;
                NetworkObject obj = HeldObj;

                onDrop.OnDrop(this);
                ItemSocket.ClearObjectBinding_Rpc(CameraManager.CamTransform.position + CameraManager.CamTransform.forward, ItemSocket.transform.rotation);

                obj.GetComponent<RigidbodyNetworkTransform>().SetLinearVelocity_Rpc(Vector3.zero);
                Vector3 force = CameraManager.CamTransform.forward;
                force *= initailThrowForce;
                force += CameraManager.CamTransform.forward * (float)context.duration * throwForceGrowthRate;

                obj.GetComponent<RigidbodyNetworkTransform>().AddForce_Rpc(force, ForceMode.Impulse);
            }
        }
    }

    public void PerformDrop(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IOnDrop useableObject))
            useableObject.OnDrop(this);

        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        Ray secondaryRay = new(CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance), Vector3.down);

        Vector3 dropPos = CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, dropDistance, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = hit.point;
            dropPos.y += dropHeight;
        }
        else if (Physics.Raycast(secondaryRay, out RaycastHit secondaryHit, dropHeight * 2, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = secondaryHit.point;
            dropPos.y += dropHeight;
        }

        ItemSocket.ClearObjectBinding_Rpc(dropPos, ItemSocket.transform.rotation, true);
    }

    public void PerformScroll(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IScroll scroll))
        {
            scroll.OnScroll(this, context);
        }
    }
    public void PerformCtrl(InputAction.CallbackContext context) 
    {
        if (HeldObj == null) return;

        if (HeldObj.TryGetComponent(out IOnCtrl ctrl)) 
        {
            ctrl.OnCtrl(this);
        }
    }


    [Rpc(SendTo.Everyone)]
    public void RequestClearItem_Rpc()
    {
        ClearItem();
    }

    public void ClearItem()
    {
        HeldObj = null;
    }

    public bool HoldingItem => HeldObj != null;
}
