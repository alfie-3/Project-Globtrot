using System;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerHoldingManager : NetworkBehaviour
{
    [field: SerializeField] public NetworkObject HeldObj { get; private set; }

    [SerializeField] public Material Material;

    [SerializeField] PlayerObjectSocketManager ObjectSocketManager;



    //Dropping
    [Space]
    [Header("Dropping")]
    [SerializeField] float dropDistance = 2.5f;
    [SerializeField] float dropHeight = 0.5f;
    [SerializeField] LayerMask dropObjectLayerMask;

    [Header("Throwing")]
    [SerializeField] AnimationCurve throwForceCurve;
    [field: SerializeField] public float maxThrowForceChargeTime { get; private set; } = 4f;
    public Action<bool> Throwing = delegate { };

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
            ObjectSocketManager.ClearAllBoundObjects();
            return;
        }

        if (obj == null) return;
        if (obj.PickedUp.Value == true) return;
        if (!obj.TryGetComponent(out NetworkObject nwObject)) return;
        HeldObj = nwObject;

        foreach (IOnHeld held in HeldObj.GetComponentsInChildren<IOnHeld>())
        {
            held.OnHeld(this);
        }

        if (obj.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
        {
            rbNWT.WakeUpNearbyObjects();
        }

        ObjectSocketManager.BindObject_Rpc(obj.HoldingSocket, HeldObj);

        GetComponentInChildren<IKTargetsManager>().ConstrainIKToObject_Rpc(HeldObj);
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

    private bool primHeld = false;
    public void PerformPrimary(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (context.interaction is HoldInteraction)
            {
                primHeld = true; NetworkManager.NetworkTickSystem.Tick += UsePrimaryOnHeldObject;
            }
            if (context.interaction is PressInteraction)
                UsePrimaryOnHeldObject();
        }
        else
        {
            if (primHeld)
            {
                primHeld = false; NetworkManager.NetworkTickSystem.Tick -= UsePrimaryOnHeldObject;
            }
        }
    }

    public void UsePrimaryOnHeldObject()
    {
        if (HeldObj == null) return;

        if (!HeldObj.TryGetComponent(out IUsePrimary useableObject)) return;

        useableObject.UsePrimary(this);
    }
    public void PerformSecondary(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        if (context.performed)
        {

            if (context.interaction is HoldInteraction)
            {
                if (!HeldObj.TryGetComponent(out IOnDrop onDrop)) return;
                throwing = true;
                Throwing.Invoke(throwing);
            }
            if (context.interaction is PressInteraction)
            {
                if (!HeldObj.TryGetComponent(out IUseSecondary useableObject)) return;
                useableObject.UseSecondary(this);
            }
        }
        else
        {

            if (throwing)
            {
                throwing = false;
                Throwing.Invoke(throwing);
                NetworkObject obj = HeldObj;

                GetComponentInChildren<IKTargetsManager>().ClearIKToObject_Rpc(HeldObj);
                ObjectSocketManager.ClearBoundObject_Rpc(HeldObj.GetComponent<Pickup_Interactable>().HoldingSocket, CameraManager.CamTransform.position + CameraManager.CamTransform.forward, ObjectSocketManager.transform.rotation);

                obj.GetComponent<RigidbodyNetworkTransform>().SetLinearVelocity_Rpc(Vector3.zero);
                Vector3 force = CameraManager.CamTransform.forward;
                //force *= Mathf.Lerp(initailThrowForce, MaxThrowForce, (float)context.duration/maxThrowForceChargeTime);
                force *= throwForceCurve.Evaluate((float)context.duration / maxThrowForceChargeTime);

                obj.GetComponent<RigidbodyNetworkTransform>().AddForce_Rpc(force, ForceMode.Impulse);

                HeldObj.GetComponentsInChildren<IOnDrop>().ToList().ForEach(x => x.OnDrop(this));
            }
        }
    }

    public void PerformDrop(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        GetComponentInChildren<IKTargetsManager>().ClearIKToObject_Rpc(HeldObj);

        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        Ray secondaryRay = new(CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance), Vector3.down);

        Vector3 dropPos = CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, dropDistance, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = hit.point;
            dropPos += hit.normal * dropHeight;
        }
        else if (Physics.Raycast(secondaryRay, out RaycastHit secondaryHit, dropHeight * 2, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = secondaryHit.point;
            dropPos.y += dropHeight;
        }

        ObjectSocketManager.ClearBoundObject_Rpc(HeldObj.GetComponent<Pickup_Interactable>().HoldingSocket, dropPos, HeldObj.transform.rotation, true);

        foreach (IOnDrop drop in HeldObj.GetComponentsInChildren<IOnDrop>())
        {
            drop.OnDrop(this);
        }
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
