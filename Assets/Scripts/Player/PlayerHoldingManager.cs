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

    //Rotation & Snapping
    [Header("Rotation")]
    [SerializeField] float snappingRotationInterval = 22.5f;
    [SerializeField] float nonSnappintRotationInterval = 8f;

    [Header("Rotation")]
    [SerializeField] float initailThrowForce = 8f;
    [SerializeField] float throwForceGrowthRate = 8f;

    public float Rotation { get; private set; }
    public bool Snapping { get; private set; }


    public PlayerCameraManager CameraManager { get; private set; }

    

    private bool throwing;

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.OnPerformPrimary += PerformPrimary;
        playerInputManager.OnPerformSecondary += PerformSecondary;
        playerInputManager.OnRotate += PerformRotate;
        playerInputManager.OnPerformDrop += PerformDrop;

        playerInputManager.OnSnapToggle += SnapToggle;

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
        Rotation = obj.transform.rotation.eulerAngles.y;

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

        /*if (throwing)
        {
            Debug.DrawRay(HeldObj.transform.position, ((CameraManager.CamTransform.forward * 16f) - HeldObj.transform.position));
        }*/
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
        IOnDrop onDrop = null;
        if (context.performed) {

            if (context.interaction is HoldInteraction) {
                if (!HeldObj.TryGetComponent(out onDrop)) return;
                throwing = true;
            }
            if (context.interaction is PressInteraction) {
                useableObject.UseSecondary(this);
            }
        } else {
            if (throwing)
            {
                throwing = false;
                if (!HeldObj.TryGetComponent(out onDrop)) return;
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

        Vector3 dropPos = CameraManager.CamTransform.position + CameraManager.CamTransform.forward * dropDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, dropDistance, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = hit.point;
            dropPos.y += dropHeight;
        }

        ItemSocket.ClearObjectBinding_Rpc(dropPos, ItemSocket.transform.rotation);
    }

    public void PerformRotate(float dir)
    {
        if (HeldObj == null) return;

        Rotation += dir * (Snapping ? snappingRotationInterval : nonSnappintRotationInterval);
    }

    public void SnapToggle(InputAction.CallbackContext context)
    {
        Snapping = !Snapping;
        if (Snapping)
        {
            float output = Mathf.Round(Rotation / snappingRotationInterval);
            if (output == 0 && Rotation > 0) output += 1;
            output *= snappingRotationInterval;
            Rotation = output;
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
        Rotation = 0;
    }

    public bool HoldingItem => HeldObj != null;
}
