using System;
using Unity.Netcode;
using UnityEngine;

public class GasFillerInputPort : NetworkBehaviour, IUseItem
{
    [SerializeField] Stock_Item referenceItem;

    public NetworkVariable<bool> Filled { get; private set; } = new();
    public Action OnGasCannisterRemoved = delegate { };

    public GameObject Cannister;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            Filled.Value = false;
    }

    public bool CanUseItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        return item == referenceItem;
    }

    public void OnItemUsed(PlayerHoldingManager manager, Stock_Item shopProduct_Item)
    {
        if (CanUseItem(manager, shopProduct_Item))
        {
            if (Filled.Value == true) return;

            NetworkObject nwObj = manager.HeldObj;
            manager.ClearHeldItem(transform.position, transform.rotation.eulerAngles);
            ConnectItem_Rpc(nwObj);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ConnectItem_Rpc(NetworkObjectReference obj)
    {
        if (!obj.TryGet(out NetworkObject nwObject)) return;

        nwObject.GetComponent<Pickup_Interactable>().OnPickedUp += CannisterRemoved;
        Cannister = nwObject.gameObject;

        nwObject.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(false);

        if (!IsServer) return;

        Filled.Value = true;
        nwObject.GetComponent<RigidbodyNetworkTransform>().Teleport(transform.position, transform.rotation, nwObject.transform.lossyScale);
    }

    public void CannisterRemoved()
    {
        if (Cannister == null) return;

        Pickup_Interactable pickup_Interactable = Cannister.GetComponent<Pickup_Interactable>();
        pickup_Interactable.OnPickedUp -= CannisterRemoved;
        OnGasCannisterRemoved.Invoke();

        Cannister.GetComponent<RigidbodyNetworkTransform>().SetRigidbodyEnabled(true);
        Cannister = null;

        if (!IsServer) return;

        Filled.Value = false;
    }

    public void SetGasCannisterType(Stock_Item item)
    {
        Cannister.GetComponent<StockItem>().SetItem(item);
    }

    public InteractionContext OnViewWithItem(PlayerHoldingManager holdingManager, Stock_Item item)
    {
        if (Filled.Value == true) return new(false);

        if (CanUseItem(holdingManager, item))
        {
            return new(true, "Insert");
        }

        return new(false);
    }

    public void OnUnview()
    {
        
    }

    private new void OnDestroy()
    {
        CannisterRemoved();
    }
}
