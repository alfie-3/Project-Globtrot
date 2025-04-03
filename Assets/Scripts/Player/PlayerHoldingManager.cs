using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerHoldingManager : NetworkBehaviour
{
    public NetworkVariable<HeldObject> NetworkedHeldObj { get; private set; } = new NetworkVariable<HeldObject>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkObject HeldObj => GetHeldObject();
    public NetworkObject GetHeldObject()
    {
        if (NetworkedHeldObj.Value == null) return null;
        if (!NetworkedHeldObj.Value.IsHolding) return null;
        if (NetworkedHeldObj.Value.NetworkObjectReference.TryGet(out NetworkObject nwObject))
        {
            return nwObject;
        }

        return null;
    }

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

        NetworkedHeldObj.OnValueChanged += HandlePlayerHolding;

        CameraManager = GetComponentInChildren<PlayerCameraManager>();
    }

    public void HandlePlayerHolding(HeldObject prev, HeldObject current)
    {
        if (!current.IsHolding)
        {
            DisconnectHeldObject(current);
        }
        else
        {
            ConnectHeldObject(current);
        }
    }

    public void HoldItem(Pickup_Interactable obj)
    {
        if (obj == null) return;
        if (obj.PickedUp.Value == true) return;
        if (!obj.TryGetComponent(out NetworkObject nwObject)) return;

        NetworkedHeldObj.Value = new(nwObject);
    }

    public void ClearHeldItem(Vector3 position, Vector3 rotation)
    {
        NetworkedHeldObj.Value = new(NetworkedHeldObj.Value.NetworkObjectReference, position, rotation);
    }

    public void ConnectHeldObject(HeldObject obj)
    {
        if (!obj.NetworkObjectReference.TryGet(out NetworkObject nwObj)) return;

        foreach (IOnHeld held in nwObj.GetComponentsInChildren<IOnHeld>())
        {
            held.OnHeld(this);
        }

        if (nwObj.TryGetComponent(out RigidbodyNetworkTransform rbNWT))
            rbNWT.WakeUpNearbyObjects();

        if (nwObj.TryGetComponent(out Pickup_Interactable pickup))
            ObjectSocketManager.BindObject_Rpc(pickup.HoldingSocket, HeldObj);

        GetComponentInChildren<IKTargetsManager>().ConstrainIKToObject(nwObj.gameObject);

    }

    public void DisconnectHeldObject(HeldObject obj)
    {
        if (!obj.NetworkObjectReference.TryGet(out NetworkObject nwObj)) return;

        foreach (IOnDrop drop in nwObj.GetComponentsInChildren<IOnDrop>())
        {
            drop.OnDrop(this);
        }

        GetComponentInChildren<IKTargetsManager>().ClearIKToObject(nwObj.gameObject);

        Transform socket = ObjectSocketManager.GetSocketTransform(nwObj.GetComponent<Pickup_Interactable>().HoldingSocket);
        ObjectSocketManager.ClearBoundObject_Rpc(nwObj.GetComponent<Pickup_Interactable>().HoldingSocket, obj.DropLocation, Quaternion.Euler(obj.DropRotation), true);

    }

    public void DisconnectAll()
    {
        ObjectSocketManager.ClearAllBoundObjects();
        GetComponentInChildren<IKTargetsManager>().ClearAllIKTargets_Rpc();
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

    public bool UsageRaycast(LayerMask layerMask, out RaycastHit rayHit)
    {
        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, 3, layerMask))
        {
            rayHit = hit;
            return true;
        }

        rayHit = default;
        return false;
    }


    public void UsePrimaryOnHeldObject()
    {
        if (!HeldObj) return;

        if (!HeldObj.TryGetComponent(out IUsePrimary useableObject)) return;

        useableObject.UsePrimary(this);
    }

    public void PerformSecondary(InputAction.CallbackContext context)
    {
        if (!HeldObj) return;

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

                obj.GetComponent<RigidbodyNetworkTransform>().SetLinearVelocity_Rpc(Vector3.zero);
                Vector3 force = CameraManager.CamTransform.forward;
                //force *= Mathf.Lerp(initailThrowForce, MaxThrowForce, (float)context.duration/maxThrowForceChargeTime);
                force *= throwForceCurve.Evaluate((float)context.duration / maxThrowForceChargeTime);

                obj.GetComponent<RigidbodyNetworkTransform>().AddForce_Rpc(force, ForceMode.Impulse);

                HeldObj.GetComponentsInChildren<IOnDrop>().ToList().ForEach(x => x.OnDrop(this));

                ClearHeldItem(CameraManager.CamTransform.position + CameraManager.CamTransform.forward, HeldObj.transform.rotation.eulerAngles);
            }
        }
    }

    public void PerformDrop(InputAction.CallbackContext context)
    {
        if (HeldObj == null) return;

        Ray ray = new(CameraManager.CamTransform.position, CameraManager.CamTransform.forward);
        Ray secondaryRay = new(CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * dropDistance);
        Debug.DrawRay(secondaryRay.origin, secondaryRay.direction * (dropHeight * 4));

        Vector3 dropPos = CameraManager.CamTransform.position + (CameraManager.CamTransform.forward * dropDistance);

        Bounds heldObjBounds = HeldObj.GetComponent<MeshRenderer>().bounds;

        if (Physics.Raycast(ray, out RaycastHit hit, dropDistance, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = hit.point;
            dropPos += hit.normal * (heldObjBounds.extents.y + dropHeight);
        }
        else if (Physics.Raycast(secondaryRay, out RaycastHit secondaryHit, dropHeight * 4, dropObjectLayerMask, QueryTriggerInteraction.Ignore))
        {
            dropPos = secondaryHit.point;
            dropPos += secondaryHit.normal * (heldObjBounds.extents.y + dropHeight);
        }

        foreach (IOnDrop drop in HeldObj.GetComponentsInChildren<IOnDrop>())
        {
            drop.OnDrop(this);
        }

        ClearHeldItem(dropPos, HeldObj.transform.rotation.eulerAngles);
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


    [Rpc(SendTo.Owner)]
    public void RequestClearItem_Rpc()
    {
        if (HeldObj == null) return;
        ClearHeldItem(HeldObj.transform.position, HeldObj.transform.rotation.eulerAngles);
    }

    public bool HoldingItem => HeldObj != null;
}

[System.Serializable]
public class HeldObject : INetworkSerializable, IEquatable<HeldObject>
{
    public NetworkObjectReference NetworkObjectReference;

    public bool IsHolding;
    public Vector3 DropLocation;
    public Vector3 DropRotation;

    public HeldObject(NetworkObjectReference reference)
    {
        NetworkObjectReference = reference;

        IsHolding = true;
    }

    public HeldObject(NetworkObjectReference reference, Vector3 pos, Vector3 rot)
    {
        NetworkObjectReference = reference;

        IsHolding = false;
        DropLocation = pos;
        DropRotation = rot;
    }

    public HeldObject() { }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();

            reader.ReadValueSafe(out NetworkObjectReference);

            reader.ReadValueSafe(out IsHolding);

            if (!IsHolding)
            {
                reader.ReadValueSafe(out DropLocation);
                reader.ReadValueSafe(out DropRotation);
            }

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();

            writer.WriteValueSafe(NetworkObjectReference);
            writer.WriteValueSafe(IsHolding);

            if (!IsHolding)
            {
                writer.WriteValueSafe(DropRotation);
                writer.WriteValueSafe(DropLocation);
            }
        }
    }

    public bool Equals(HeldObject other)
    {
        return other.NetworkObjectReference.Equals(NetworkObjectReference) && IsHolding == other.IsHolding;
    }
}